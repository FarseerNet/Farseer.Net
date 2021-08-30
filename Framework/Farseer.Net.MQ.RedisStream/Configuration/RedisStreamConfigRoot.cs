using System.Collections.Generic;
using System.Linq;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.RedisStream.Configuration
{
    /// <summary>
    /// 读取RedisStream配置
    /// </summary>
    public class RedisStreamConfigRoot
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        public static List<RedisStreamConfig> Get()
        {
            var configurationSection = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("RedisStream");
            return configurationSection.GetChildren().Select(o => o.Get<RedisStreamConfig>()).ToList();
        }
    }
}