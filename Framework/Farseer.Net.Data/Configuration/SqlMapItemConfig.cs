using System;

namespace FS.Data.Configuration
{
    /// <summary> SQL语句配置 </summary>
    [Serializable]
    public class SqlMapItemConfig
    {
        /// <summary> 映射SqlSet的命名空间 + 属性名称 </summary>
        public string Name { get; set; }

        /// <summary> SQL语句串 </summary>
        public string Sql { get; set; }
    }
}