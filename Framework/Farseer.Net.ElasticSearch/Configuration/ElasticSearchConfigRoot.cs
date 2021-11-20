using System.Collections.Generic;
using System.Linq;
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
            var configurationSection = IocManager.GetService<IConfigurationRoot>().GetSection(key: "ElasticSearch");
            return configurationSection.GetChildren().Select(selector: o => o.Get<ElasticSearchItemConfig>()).ToList();
        }
    }
}