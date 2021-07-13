using System;
using System.Collections.Generic;
using FS.Configuration;

namespace FS.Data.Configuration
{
    /// <summary> 数据库连接配置 </summary>
    [Serializable]
    public class DbConfig
    {
        /// <summary> 数据库连接配置列表 </summary>
        public List<DbItemConfig> Items { get; set; }
    }
}