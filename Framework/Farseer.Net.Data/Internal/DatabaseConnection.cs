using FS.Data.Inteface;

namespace FS.Data.Internal
{
    /// <summary>
    ///     上下文数据库连接信息
    /// </summary>
    public class DatabaseConnection : IDatabaseConnection
    {
        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="connectionString"> 连接字符串 </param>
        /// <param name="dbType"> 数据库类型 </param>
        /// <param name="commandTimeout"> 命令超时时间 </param>
        /// <param name="dataVer"> 数据库版本 </param>
        public DatabaseConnection(string connectionString, eumDbType dbType, int commandTimeout, string dataVer)
        {
            ConnectionString = connectionString;
            DbType           = dbType;
            CommandTimeout   = commandTimeout;
            DataVer          = dataVer;
        }

        /// <summary>
        ///     连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     数据库类型
        /// </summary>
        public eumDbType DbType { get; set; }

        /// <summary>
        ///     命令超时时间
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        ///     数据库版本
        /// </summary>
        public string DataVer { get; set; }
    }
}