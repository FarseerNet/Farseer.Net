using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farseer.Net.Configuration;

namespace Farseer.Net.ElasticSearch.Configuration
{
    /// <summary>
    /// ES配置类，支持多个集群配置
    /// </summary>
    public class ElasticSearchConfig : IFarseerConfig
    {
        public List<ElasticSearchItemConfig> Items = new List<ElasticSearchItemConfig>();
    }
}
