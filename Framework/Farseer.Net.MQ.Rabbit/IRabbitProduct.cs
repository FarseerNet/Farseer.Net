using System;
using System.Collections.Generic;
using FS.Core.Abstract.MQ;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    public interface IRabbitProduct : IMqProduct
    {
        /// <summary>
        ///     发送消息（routingKey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        bool Send(string message, Action<IBasicProperties> funcBasicProperties = null);

        /// <summary>
        ///     发送消息（批量）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="exchange"> 交换器名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        bool Send(IEnumerable<string> message, string routingKey, string exchange, Action<IBasicProperties> funcBasicProperties = null);

        /// <summary>
        ///     发送消息（routingKey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        bool Send(IEnumerable<string> message, Action<IBasicProperties> funcBasicProperties = null);

        /// <summary>
        ///     发送消息（routingKey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        bool Send(string message, string routingKey, Action<IBasicProperties> funcBasicProperties = null);

        /// <summary>
        ///     发送消息（routingKey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        bool Send(IEnumerable<string> message, string routingKey, Action<IBasicProperties> funcBasicProperties = null);
    }
}