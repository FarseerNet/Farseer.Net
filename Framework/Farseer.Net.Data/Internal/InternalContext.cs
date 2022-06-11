using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Collections.Pooled;
using FS.Data.Abstract;
using FS.Data.Client;
using FS.Data.Data;
using FS.DI;

namespace FS.Data.Internal
{
    /// <summary>
    ///     数据库上下文初始化程序
    /// </summary>
    internal class InternalContext : IDisposable
    {
        // 用于实现事务的作用域，在同一个作用域、连接字符串下，共享事务。
        private static readonly PooledDictionary<string, AsyncLocal<InternalContext>> scopeContext = new();

        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="contextType"> 父类上下文类型 </param>
        public InternalContext(Type contextType)
        {
            ContextType = contextType;
            ScopeID     = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 当前作用域的上下文
        /// </summary>
        internal AsyncLocal<InternalContext> CurrentScope => scopeContext[DatabaseConnection.ConnectionString];

        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        internal IDatabaseConnection DatabaseConnection { get; private set; }

        /// <summary>
        /// 作用域唯一标识
        /// </summary>
        internal string ScopeID { get; }

        /// <summary>
        ///     外部上下文类型
        /// </summary>
        public Type ContextType { get; }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        public AbsDbProvider DbProvider => IocManager.GetService<AbsDbProvider>($"dbProvider_{DatabaseConnection.DbType}");

        /// <summary>
        ///     当事务提交后，会调用该委托
        /// </summary>
        private PooledList<Action> _commitCallback = new();

        /// <summary>
        ///     执行数据库操作
        /// </summary>
        public IExecuteSql ExecuteSql { get; private set; }

        /// <summary>
        ///     队列管理
        /// </summary>
        public QueryManger QueryManger { get; private set; }

        /// <summary>
        ///     手动编写SQL
        /// </summary>
        public ManualSql ManualSql { get; private set; }
        /// <summary>
        /// 数据库执行者
        /// </summary>
        public DbExecutor DbExecutor { get; private set; }

        /// <summary>
        ///     初始化数据库环境、实例化子类中，所有Set属性
        /// </summary>
        public void Initializer()
        {
            if (!scopeContext.ContainsKey(DatabaseConnection.ConnectionString))
            {
                scopeContext[DatabaseConnection.ConnectionString] = new();
            }
            CurrentScope.Value ??= this;

            // 说明调用前已开启了事务
            if (CurrentScope.Value.ScopeID != ScopeID)
            {
                //  连接字符串
                DatabaseConnection = CurrentScope.Value.DatabaseConnection;
                // 手动编写SQL
                ManualSql = CurrentScope.Value.ManualSql;
                // 默认SQL执行者
                ExecuteSql  = CurrentScope.Value.ExecuteSql;
                DbExecutor = CurrentScope.Value.DbExecutor;
                // 队列管理者
                QueryManger = new QueryManger(CurrentScope.Value);

                //Console.WriteLine($"我使用主域的事务：当前：{ScopeID}，主域：{CurrentScope.Value.ScopeID}");
            }
            else
            {
                // 数据库的事务级别
                var tranLevel = DbProvider.IsSupportTransaction ? IsolationLevel.RepeatableRead : IsolationLevel.Unspecified;

                // 默认SQL执行者
                DbExecutor = new DbExecutor(connectionString: DatabaseConnection.ConnectionString, dbType: DatabaseConnection.DbType, commandTimeout: DatabaseConnection.CommandTimeout, tranLevel: tranLevel, dbProvider: DbProvider);
                ExecuteSql  = new ExecuteSql(contextProvider: this);

                // 队列管理者
                QueryManger = new QueryManger(provider: this);
                // 手动编写SQL
                ManualSql = new ManualSql(context: this);

                //Console.WriteLine($"我使用自己的事务：当前：{ScopeID}");
            }
        }

        /// <summary>
        /// 使用工作单元模式
        /// </summary>
        public void UseUnitOfWork()
        {
            // 在同一作用域下，说明外层没有开启事务，则这里需取消事务
            if (CurrentScope.Value?.ScopeID == ScopeID)
            {
                CurrentScope.Value = null;
                DbExecutor.CancelTran();
            }
        }

        /// <summary>
        /// 完成事务后，需清空当前事务作用域、关闭事务
        /// </summary>
        public void FinishTransaction()
        {
            // 重置作用域，并将回调列表复制到本地
            if (CurrentScope.Value != null)
            {
                _commitCallback    = CurrentScope.Value._commitCallback;
                CurrentScope.Value = null;
            }
            DbExecutor.CancelTran();
            DbExecutor.Close(dispose: true);
        }

        /// <summary>
        /// 设置数据库连接信息
        /// </summary>
        internal void SetDatabaseConnection(string dbConfigName)
        {
            var dbConnectionName = $"dbConnection_{dbConfigName}";
            if (!IocManager.Instance.IsRegistered(name: dbConnectionName)) throw new FarseerException(message: $"未找到数据库的配置：{dbConfigName}");

            DatabaseConnection = IocManager.GetService<IDatabaseConnection>(name: dbConnectionName);
        }

        /// <summary>
        /// 设置数据库连接信息
        /// </summary>
        internal void SetDatabaseConnection(IDatabaseConnection databaseConnection)
        {
            DatabaseConnection = databaseConnection;
        }

        /// <summary>
        /// 设置数据库连接信息
        /// </summary>
        internal void SetDatabaseConnection(string connectionString, eumDbType db, int commandTimeout, string dataVer)
        {
            DatabaseConnection = new DatabaseConnection(connectionString: connectionString, dbType: db, commandTimeout: commandTimeout, dataVer: dataVer);
        }

        /// <summary>
        ///     当事务提交后，会调用该委托
        /// </summary>
        public void AddCallback(Action act)
        {
            // 添加到主事务中
            CurrentScope.Value._commitCallback.Add(item: act);
        }

        /// <summary>
        ///     当事务提交后，执行回调
        /// </summary>
        public void ExecuteCallback()
        {
            // 这里不用CurrentScope.Value._commitCallback，因为前面重置作用域时，已复制到本地变量中
            foreach (var action in _commitCallback) action();

            _commitCallback.Dispose();
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        [EditorBrowsable(state: EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing"> 是否释放托管资源 </param>
        [EditorBrowsable(state: EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                FinishTransaction();
                QueryManger.Dispose();
                ExecuteSql.Dispose();
                DbExecutor.Dispose();
                _commitCallback.Dispose();
            }
        }
    }
}