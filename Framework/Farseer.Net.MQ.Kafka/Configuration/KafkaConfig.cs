using System.Collections.Generic;
using Farseer.Net.Configuration;

namespace Farseer.Net.MQ.Kafka.Configuration
{
    /// <summary>
    /// Kafka配置,支持多个集群配置
    /// </summary>
    public class KafkaConfig : IFarseerConfig
    {
        /// <summary>
        /// 多个配置集合
        /// </summary>
        public List<KafkaItemConfig> Items { get; set; } = new List<KafkaItemConfig>();
    }
}
