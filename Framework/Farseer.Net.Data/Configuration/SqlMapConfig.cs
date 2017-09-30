using System;
using System.Collections.Generic;
using Farseer.Net.Configuration;

namespace Farseer.Net.Data.Configuration
{
    /// <summary> SQL语句配置 </summary>
    [Serializable]
    public class SqlMapConfig: IFarseerConfig
    {
        /// <summary> SQL语句配置列表 </summary>
        public readonly List<SqlMapItemConfig> Items = new List<SqlMapItemConfig>();
    }
}