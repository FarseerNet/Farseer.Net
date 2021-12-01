using System.Collections.Generic;
using FS.Cache.Redis.Configuration;

namespace FS.MQ.RedisStream.Configuration
{
    /// <summary>
    ///     RocketMQ配置信息
    /// </summary>
    public class RedisStreamConfig
    {
        /// <summary>
        ///     Redis配置
        /// </summary>
        public RedisItemConfig Server { get; set; }
        
        /// <summary>
        ///     生产者配置
        /// </summary>
        public List<ProductItemConfig> Product { get; set; }
    }
}