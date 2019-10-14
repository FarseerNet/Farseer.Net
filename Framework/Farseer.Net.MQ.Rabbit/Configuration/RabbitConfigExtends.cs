using System.Collections.Generic;
using FS.MQ.RabbitMQ.Configuration;

// ReSharper disable once CheckNamespace
namespace FS.Configuration
{
    /// <summary>
    ///     RocketMQ配置扩展
    /// </summary>
    public static class RabbitConfigExtends
    {
        /// <summary>
        ///     获取配置文件
        /// </summary>
        public static RabbitConfig RabbitConfig(this IConfigResolver resolver)
        {
            return resolver.Get<RabbitConfig>() ?? new RabbitConfig {Items = new List<RabbitItemConfig>()};
        }
    }
}