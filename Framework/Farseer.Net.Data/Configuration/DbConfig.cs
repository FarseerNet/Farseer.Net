using System;
using System.Collections.Generic;
using Farseer.Net.Configuration;

namespace Farseer.Net.Data.Configuration
{
    /// <summary> 数据库连接配置 </summary>
    [Serializable]
    public class DbConfig : IFarseerConfig
    {
        /// <summary> 是否把输出SQL记录 </summary>
        public bool IsWriteSqlRunLog { get; set; }

        /// <summary> 是否把输出SQL执行异常的记录 </summary>
        public bool IsWriteSqlErrorLog { get; set; }

        /// <summary> 数据库连接配置列表 </summary>
        public List<DbItemConfig> Items = new List<DbItemConfig>();
    }
}