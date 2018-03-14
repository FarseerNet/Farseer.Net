using FS.Data.Infrastructure;

namespace FS.Data
{
    /// <summary>
    ///     多张表带静态实例化的上下文
    /// </summary>
    /// <typeparam name="TPo"></typeparam>
    public class DbContext<TPo> : DbContext where TPo : DbContext<TPo>, new()
    {
        /// <summary>
        ///     通过数据库配置，连接数据库
        /// </summary>
        /// <param name="name">数据库配置名称</param>
        /// <param name="isUnitOfWork">是否工作单元模式</param>
        protected DbContext(string name, bool isUnitOfWork = false) : base(name, isUnitOfWork) { }

        /// <summary>
        ///     通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="db">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected DbContext(string connectionString, eumDbType db, int commandTimeout = 30) : base(connectionString, db, commandTimeout) { }

        /// <summary>
        ///     静态实例
        /// </summary>
        public static TPo Data => Data<TPo>();

        /// <summary>
        ///     创建来自其它上下文的共享
        /// </summary>
        /// <param name="otherContext">其它上下文（主上下文）</param>
        public static TPo TransactionInstance(DbContext otherContext) => TransactionInstance<TPo>(otherContext);
    }
}