using System.Collections.Generic;
using Farseer.Net.Configuration;
using Farseer.Net.MQ.Kafka.Configuration;

// ReSharper disable once CheckNamespace
namespace Farseer.Net.Configuration
{
    /// <summary>
    /// Kafka配置
    /// </summary>
    public static class KafkaConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static KafkaConfig KafkaConfig(this IConfigResolver resolver) => resolver.Get<KafkaConfig>() ?? new KafkaConfig { Items = new List<KafkaItemConfig>() };
    }
}
