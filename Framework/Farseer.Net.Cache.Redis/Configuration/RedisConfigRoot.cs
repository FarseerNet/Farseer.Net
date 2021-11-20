using System.Collections.Generic;
using System.Linq;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Cache.Redis.Configuration
{
    /// <summary>
    ///     读取Redis配置
    /// </summary>
    public class RedisConfigRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static List<RedisItemConfig> Get()
        {
            var configurationSection = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Redis");
            return configurationSection.GetChildren().Select(selector: o => o.Get<RedisItemConfig>()).ToList();
        }
    }
}