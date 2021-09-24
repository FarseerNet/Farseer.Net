using System.Collections.Generic;
using System.Data;

namespace FS.Core.LinkTrack
{
    public class DbLinkTrackDetail
    {
        /// <summary>
        ///     连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///     数据库名称
        /// </summary>
        public string DataBaseName { get; set; }

        /// <summary>
        ///     表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        ///     执行方式
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        ///     SQL文本
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        ///     SQL参数化
        /// </summary>
        public Dictionary<string, string> SqlParam { get; set; }
    }
}