using System.Collections.Generic;
using System.Linq;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Cache.Redis.Configuration
{
    /// <summary>
    /// 读取Redis配置
    /// </summary>
    public class RedisConfigRoot
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        public static List<RedisItemConfig> Get()
        {
            var configurationSection = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("Redis");
            return configurationSection.GetChildren().Select(o => o.Get<RedisItemConfig>()).ToList();
        }
    }
}