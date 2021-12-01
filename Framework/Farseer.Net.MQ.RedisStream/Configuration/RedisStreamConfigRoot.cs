using System.Collections.Generic;
using System.Linq;
using FS.Cache.Redis.Configuration;
using FS.Core.Configuration;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.RedisStream.Configuration
{
    /// <summary>
    ///     读取RedisStream配置
    /// </summary>
    public class RedisStreamConfigRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static List<RedisStreamConfig> Get()
        {
            var configs   = IocManager.GetService<IConfigurationRoot>().GetSection(key: "RedisStream").GetChildren();
            var lstConfig = new List<RedisStreamConfig>();
            foreach (var configurationSection in configs)
            {
                var config = new RedisStreamConfig
                {
                    Product = new(),
                    Server  = ConfigConvert.ToEntity<RedisItemConfig>(configurationSection.GetSection("Server").Value) // 服务器配置
                };
                if (config.Server == null) continue;
                config.Server.Name = configurationSection.Key;
                lstConfig.Add(config);
                
                // 生产者配置
                var products = configurationSection.GetSection("Product").GetChildren();
                foreach (var product in products)
                {
                    var productItemConfig = ConfigConvert.ToEntity<ProductItemConfig>(product.Value);
                    if (productItemConfig == null) continue;
                    productItemConfig.QueueName = product.Key;
                    config.Product.Add(productItemConfig);
                }
            }
            return lstConfig;
        }
    }
}