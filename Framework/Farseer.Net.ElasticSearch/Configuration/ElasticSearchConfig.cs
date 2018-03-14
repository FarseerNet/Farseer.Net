using System.Collections.Generic;
using FS.Configuration;

namespace FS.ElasticSearch.Configuration
{
    /// <summary>
    /// ES配置类，支持多个集群配置
    /// </summary>
    public class ElasticSearchConfig : IFarseerConfig
    {
        public List<ElasticSearchItemConfig> Items = new List<ElasticSearchItemConfig>();
    }
}
