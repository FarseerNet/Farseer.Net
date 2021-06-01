using System;
using System.Collections.Generic;
using FS.Configuration;

namespace FS.Data.Configuration
{
    /// <summary> 数据库连接配置 </summary>
    [Serializable]
    public class DbConfig : IFarseerConfig
    {
        /// <summary> 是否把输出SQL记录 </summary>
        public bool IsWriteSqlRunLog { get; set; }

        /// <summary> 是否把打印SQL记录 </summary>
        public bool IsPrintSqlLog { get; set; }

        /// <summary> 数据库连接配置列表 </summary>
        public List<DbItemConfig> Items { get; set; }
    }
}