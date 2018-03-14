using System.Collections.Generic;
using FS.MQ.RocketMQ.Configuration;

// ReSharper disable once CheckNamespace

namespace FS.Configuration
{
    /// <summary>
    ///     RocketMQ配置扩展
    /// </summary>
    public static class RocketMQConfigExtends
    {
        /// <summary>
        ///     获取配置文件
        /// </summary>
        public static RocketMQConfig RocketMQConfig(this IConfigResolver resolver) => resolver.Get<RocketMQConfig>() ?? new RocketMQConfig {Items = new List<RocketMQItemConfig>()};
    }
}