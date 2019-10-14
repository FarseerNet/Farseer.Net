using System.Collections.Generic;
using FS.Configuration;

namespace FS.MQ.RabbitMQ.Configuration
{
    /// <summary>
    ///     RabbitMQ配置,支持多个集群配置
    /// </summary>
    public class RabbitConfig : IFarseerConfig
    {
        /// <summary>
        ///     多个配置集合
        /// </summary>
        public List<RabbitItemConfig> Items { get; set; } = new List<RabbitItemConfig>();
    }
}