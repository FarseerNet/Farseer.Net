using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS.Core.Configuration;
using FS.DI;
using FS.Extends;
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
        public static IEnumerable<RabbitItemConfig> Get()
        {
            using var configs   = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Rabbit").GetChildren().ToPooledList();
            foreach (var configurationSection in configs)
            {
                var config = new RabbitItemConfig
                {
                    Product = new(),
                    Server  = ConfigConvert.ToEntity<RabbitServerConfig>(configurationSection.GetSection("Server").Value) // 服务器配置
                };
                if (config.Server == null) continue;
                config.Server.Name = configurationSection.Key;
                if (config.Server.Server.Contains(":"))
                {
                    var servers = config.Server.Server.Split(':');
                    config.Server.Server = servers[0];
                    config.Server.Port   = servers[1].ConvertType(-1);
                }
                
                // 生产者配置
                using var products = configurationSection.GetSection("Product").GetChildren().ToPooledList();
                foreach (var product in products)
                {
                    var productItemConfig = ConfigConvert.ToEntity<ProductConfig>(product.Value);
                    if (productItemConfig == null) continue;
                    productItemConfig.Name = product.Key;
                    config.Product.Add(productItemConfig);
                }

                yield return config;
            }
        }
    }
}