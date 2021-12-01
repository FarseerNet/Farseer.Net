using System.Collections.Generic;
using System.Linq;
using FS.Core.Configuration;
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
            var configs   = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Redis").GetChildren();
            var lstConfig = new List<RedisItemConfig>();
            foreach (var configurationSection in configs)
            {
                var config = ConfigConvert.ToEntity<RedisItemConfig>(configurationSection.Value);
                if (config == null) continue;
                config.Name = configurationSection.Key;
                lstConfig.Add(config);
            }
            return lstConfig;
        }
    }
}