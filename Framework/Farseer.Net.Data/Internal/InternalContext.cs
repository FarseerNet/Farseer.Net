using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using FS.Data.Client;
using FS.Data.Data;
using FS.Data.Infrastructure;
using FS.DI;

namespace FS.Data.Internal
{
    /// <summary>
    ///     数据库上下文初始化程序
    /// </summary>
    internal class InternalContext : IDisposable
    {
        // 用于实现事务的作用域，在同一个作用域、连接字符串下，共享事务。
        private static readonly Dictionary<string, AsyncLocal<InternalContext>> scopeContext = new();
        //private static readonly AsyncLocal<InternalContext> scopeContext = new();

        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="contextType"> 父类上下文类型 </param>
        public InternalContext(Type contextType)
        {
            ContextType = contextType;
            ScopeID     = Guid.NewGuid().ToString("N");
        }

        private AsyncLocal<InternalContext> CurrentScope => scopeContext[DatabaseConnection.ConnectionString];
        
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
        public Type ContextType { get; private set; }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        public AbsDbProvider DbProvider => IocManager.GetService<AbsDbProvider>($"dbProvider_{DatabaseConnection.DbType}");

        /// <summary>
        ///     执行数据库操作
        /// </summary>
        public IExecuteSql Executeor { get; private set; }

        /// <summary>
        ///     队列管理
        /// </summary>
        public QueueManger QueueManger { get; private set; }

        /// <summary>
        ///     手动编写SQL
        /// </summary>
        public ManualSql ManualSql { get; private set; }

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
                Executeor = CurrentScope.Value.Executeor;
                // 队列管理者
                QueueManger = new QueueManger(CurrentScope.Value);
            }
            else
            {
                // 数据库的事务级别
                var tranLevel = DbProvider.IsSupportTransaction ? IsolationLevel.RepeatableRead : IsolationLevel.Unspecified;

                // 默认SQL执行者
                Executeor = new ExecuteSql(dataBase: new DbExecutor(connectionString: DatabaseConnection.ConnectionString, dbType: DatabaseConnection.DbType, commandTimeout: DatabaseConnection.CommandTimeout, tranLevel: tranLevel, dbProvider: DbProvider), contextProvider: this);

                // 记录执行链路（通过代理模式实现）
                Executeor = new ExecuteSqlMonitorProxy(db: Executeor, dbProvider: DbProvider);

                // 队列管理者
                QueueManger = new QueueManger(provider: this);
                // 手动编写SQL
                ManualSql = new ManualSql(context: this);
            }
        }

        /// <summary>
        /// 使用工作单元模式
        /// </summary>
        public void UseUnitOfWork()
        {
            // 在同一作用域下，说明外层没有开启事务，则这里需取消事务
            if (CurrentScope.Value.ScopeID == ScopeID)
            {
                CurrentScope.Value = null;
                Executeor.DataBase.CloseTran();
            }
        }

        /// <summary>
        /// 完成事务后，需清空当前事务作用域、关闭事务
        /// </summary>
        public void FinishTransaction()
        {
            CurrentScope.Value = null;
            Executeor.DataBase.CloseTran();
            Executeor.DataBase.Close(dispose: true);
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
                QueueManger.Dispose();
                Executeor.DataBase.Dispose();
            }
        }
    }
}