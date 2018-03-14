using System;
using System.Collections.Generic;
using FS.Configuration;

namespace FS.Data.Configuration
{
    /// <summary> SQL语句配置 </summary>
    [Serializable]
    public class SqlMapConfig: IFarseerConfig
    {
        /// <summary> SQL语句配置列表 </summary>
        public readonly List<SqlMapItemConfig> Items = new List<SqlMapItemConfig>();
    }
}