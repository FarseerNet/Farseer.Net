using System.Collections.Generic;
using System.Linq;
using FS.Core.Configuration;
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
            var configs   = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Rabbit").GetChildren();
            var lstConfig = new List<RabbitItemConfig>();
            foreach (var configurationSection in configs)
            {
                var config = new RabbitItemConfig
                {
                    Product = new(),
                    Server  = ConfigConvert.ToEntity<RabbitServerConfig>(configurationSection.GetSection("Server").Value) // 服务器配置
                };
                if (config.Server == null) continue;
                config.Server.Name = configurationSection.Key;
                lstConfig.Add(config);

                // 生产者配置
                var products = configurationSection.GetSection("Product").GetChildren();
                foreach (var product in products)
                {
                    var productItemConfig = ConfigConvert.ToEntity<ProductConfig>(product.Value);
                    if (productItemConfig == null) continue;
                    productItemConfig.Name = product.Key;
                    config.Product.Add(productItemConfig);
                }
            }
            return lstConfig;
        }
    }
}