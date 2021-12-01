using System.Collections.Generic;
using FS.Core.Configuration;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.ElasticSearch.Configuration
{
    /// <summary>
    ///     读取ElasticSearch配置
    /// </summary>
    public class ElasticSearchConfigRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static List<ElasticSearchItemConfig> Get()
        {
            var configs   = IocManager.GetService<IConfigurationRoot>().GetSection(key: "ElasticSearch").GetChildren();
            var lstConfig = new List<ElasticSearchItemConfig>();
            foreach (var configurationSection in configs)
            {
                var config = ConfigConvert.ToEntity<ElasticSearchItemConfig>(configurationSection.Value);
                if (config == null) continue;
                config.Name = configurationSection.Key;
                lstConfig.Add(config);
            }
            return lstConfig;
        }
    }
}