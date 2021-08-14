using System;
using System.ComponentModel;
using System.Data;
using FS.Configuration;
using FS.Core;
using FS.Data.Client;
using FS.Data.Data;
using FS.Data.Infrastructure;
using FS.Data.Map;
using FS.DI;

namespace FS.Data.Internal
{
    /// <summary>
    ///     数据库上下文初始化程序
    /// </summary>
    internal class InternalContext : IDisposable
    {
        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="contextType">外部上下文类型</param>
        /// <param name="isUnitOfWork">是否启用单元工作模式</param>
        /// <param name="contextConnection">上下文数据库连接信息</param>
        public InternalContext(Type contextType, bool isUnitOfWork, ContextConnection contextConnection = null)
        {
            this.ContextType = contextType;
            this.IsUnitOfWork = isUnitOfWork;
            this.ContextConnection = contextConnection;
        }

        /// <summary>
        ///     初始化数据库环境（共享自其它上下文）、实例化子类中，所有Set属性
        /// </summary>
        /// <param name="currentContextType">外部上下文类型</param>
        /// <param name="masterContext">其它上下文（主上下文）</param>
        public void TransactionInstance(Type currentContextType, InternalContext masterContext)
        {
            this.ContextType = currentContextType;
            this.IsUnitOfWork = masterContext.IsUnitOfWork;
            //  连接字符串
            this.ContextConnection = masterContext.ContextConnection;
            // 手动编写SQL
            ManualSql = masterContext.ManualSql;
            // 默认SQL执行者
            Executeor = masterContext.Executeor;
            // 数据库提供者
            DbProvider = masterContext.DbProvider;
            // 队列管理者
            QueueManger = new QueueManger(masterContext); //masterContext.QueueManger;


            // 上下文映射关系
            ContextMap = new ContextDataMap(ContextType);
            // 不再需要初始化
            IsInitializer = true;
        }

        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        internal IContextConnection ContextConnection { get; set; }
        /// <summary>
        ///     外部上下文类型
        /// </summary>
        public Type ContextType { get; private set; }

        /// <summary>
        ///     是否初始化
        /// </summary>
        public bool IsInitializer { get; private set; }

        /// <summary>
        ///     是否初始化实体类名（表名、视图、存储过程）
        /// </summary>
        public bool IsInitModelName { get; internal set; }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        public AbsDbProvider DbProvider { get; private set; }

        /// <summary>
        ///     执行数据库操作
        /// </summary>
        public IExecuteSql Executeor { get; private set; }

        /// <summary>
        ///     映射结构关系
        /// </summary>
        public ContextDataMap ContextMap { get; private set; }

        /// <summary>
        ///     队列管理
        /// </summary>
        public QueueManger QueueManger { get; private set; }

        /// <summary>
        ///     true:立即执行，不需要调用SaveChange()方法 执行
        ///     false:启用合并执行命令、并延迟加载，执行完后，需要调用SaveChange()方法
        /// </summary>
        public bool IsUnitOfWork { get; internal set; }

        /// <summary>
        ///     手动编写SQL
        /// </summary>
        public ManualSql ManualSql { get; internal set; }

        /// <summary>
        ///     初始化数据库环境、实例化子类中，所有Set属性
        /// </summary>
        public void Initializer()
        {
            if (IsInitializer) { return; }

            // 数据库提供者
            DbProvider = AbsDbProvider.CreateInstance(ContextConnection.DbType, ContextConnection.DataVer);
            
            // 默认SQL执行者
            Executeor = new ExecuteSql(new DbExecutor(ContextConnection.ConnectionString, ContextConnection.DbType, ContextConnection.CommandTimeout, !IsUnitOfWork && DbProvider.IsSupportTransaction ? IsolationLevel.RepeatableRead : IsolationLevel.Unspecified, DbProvider), this);

            // 记录执行链路
            Executeor = new ExecuteSqlMonitorProxy(Executeor, DbProvider);
            
            // 队列管理者
            QueueManger = new QueueManger(this);
            // 手动编写SQL
            ManualSql = new ManualSql(this);
            // 上下文映射关系
            this.ContextMap = new ContextDataMap(ContextType);

            IsInitializer = true;
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.Dispose();
                Executeor.DataBase.Dispose();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}