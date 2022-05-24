using FS.Data.Infrastructure;

namespace FS.Data.Internal
{
    public interface IDatabaseConnection
    {
        /// <summary>
        ///     连接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        ///     数据库类型
        /// </summary>
        eumDbType DbType { get; set; }

        /// <summary>
        ///     命令超时时间
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        ///     数据库版本
        /// </summary>
        string DataVer { get; set; }
    }
}