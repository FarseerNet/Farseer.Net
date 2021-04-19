using System.Collections.Generic;

namespace FS.MQ.RabbitMQ.Configuration
{
    /// <summary>
    ///     RocketMQ配置信息
    /// </summary>
    public class RabbitItemConfig : RabbitServerConfig
    {
        /// <summary> 生产者配置 </summary>
        public List<ProductConfig> Product { get; set; }
    }
}