using System.Collections.Generic;
using System.Linq;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.Rabbit.Configuration
{
    /// <summary>
    ///     读取Rabbit配置
    /// </summary>
    public class RabbitConfigRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static List<RabbitItemConfig> Get()
        {
            var configurationSection = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Rabbit");
            return configurationSection.GetChildren().Select(selector: o => o.Get<RabbitItemConfig>()).ToList();
        }
    }
}