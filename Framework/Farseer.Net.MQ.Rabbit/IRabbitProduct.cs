using System.Collections.Generic;
using RabbitMQ.Client;

namespace FS.MQ.RabbitMQ
{
    public interface IRabbitProduct
    {
        /// <summary>
        ///     关闭生产者
        /// </summary>
        void Close();

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="basicProperties">属性</param>
        bool Send(string message, IBasicProperties basicProperties = null);

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="routingKey">路由KEY名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="basicProperties">属性</param>
        bool Send(string message, string routingKey, string exchange = "", IBasicProperties basicProperties = null);

        /// <summary>
        ///     发送消息（批量）
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="routingKey">路由KEY名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="basicProperties">属性</param>
        bool Send(IEnumerable<string> message, string routingKey, string exchange = "", IBasicProperties basicProperties = null);

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="basicProperties">属性</param>
        bool Send(IEnumerable<string> message, IBasicProperties basicProperties = null);
    }
}