using System.Collections.Generic;
using Farseer.Net.Configuration;

namespace Farseer.Net.MQ.RocketMQ.Configuration
{
    /// <summary>
    ///     RocketMQ配置,支持多个集群配置
    /// </summary>
    public class RocketMQConfig : IFarseerConfig
    {
        /// <summary>
        ///     多个配置集合
        /// </summary>
        public List<RocketMQItemConfig> Items { get; set; } = new List<RocketMQItemConfig>();
    }
}