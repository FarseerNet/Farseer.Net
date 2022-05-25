using FS.Core.Data;
using FS.Data.Infrastructure;

namespace FS.Data
{
    /// <summary>
    ///     多张表带静态实例化的上下文
    /// </summary>
    /// <typeparam name="TDbContext"> </typeparam>
    public class DbContext<TDbContext> : DbContext where TDbContext : DbContext<TDbContext>, new()
    {
        /// <summary>
        ///     动态分库方案，此时需要重写：SplitDatabase方法
        /// </summary>
        protected DbContext() : base(null)
        {
        }
        
        /// <summary>
        ///     通过数据库配置，连接数据库
        /// </summary>
        /// <param name="name"> 数据库配置名称 </param>
        protected DbContext(string name) : base(name)
        {
        }

        /// <summary>
        ///     通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString"> 数据库连接字符串 </param>
        /// <param name="db"> 数据库类型 </param>
        /// <param name="commandTimeout"> SQL执行超时时间 </param>
        protected DbContext(string connectionString, eumDbType db, int commandTimeout = 30) : base(connectionString: connectionString, db: db, commandTimeout: commandTimeout)
        {
        }

        /// <summary>
        ///     静态实例
        /// </summary>
        public static TDbContext Data => Data<TDbContext>();
    }
}