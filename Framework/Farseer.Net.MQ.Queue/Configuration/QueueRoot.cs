using System.Collections.Generic;
using FS.Core.Configuration;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.Queue.Configuration
{
    /// <summary>
    ///     读取配置
    /// </summary>
    public class QueueRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static IEnumerable<QueueConfig> Get()
        {
            var configs = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Queue").GetChildren();
            foreach (var configurationSection in configs)
            {
                var config = ConfigConvert.ToEntity<QueueConfig>(configurationSection.Value);
                if (config == null) continue;
                config.Name = configurationSection.Key;
                yield return config;
            }
        }
    }
}