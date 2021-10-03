using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.DI;
using FS.MQ.Rabbit.Attr;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     Rabbit监听消费
    /// </summary>
    public interface IListenerMessage
    {
        /// <summary>
        ///     消费
        /// </summary>
        /// <returns> 当开启手动确认时，返回true时，才会进行ACK确认 </returns>
        Task<bool> Consumer(string message, object sender, BasicDeliverEventArgs ea);

        /// <summary>
        ///     当异常时处理
        /// </summary>
        /// <returns> true：表示成功处理，移除消息。false：处理失败，如果是重试状态，则放回队列 </returns>
        Task<bool> FailureHandling(string message, object sender, BasicDeliverEventArgs ea) => Task.FromResult(result: false);

        public Task Init(IIocManager iocManager, ConsumerAttribute consumerAtt, Type consumerType)
        {
            // 读取配置
            var rabbitItemConfig = RabbitConfigRoot.Get().Find(match: o => o.Name == consumerAtt.Name);
            if (rabbitItemConfig == null)
            {
                iocManager.Logger<IListenerMessageBatch>().LogWarning(message: $"未找到：{consumerType.FullName}的配置项：{consumerAtt.Name}");
                return Task.FromResult(result: 0);
            }

            // 启用启动绑定时，要创建交换器、队列，并绑定
            if (consumerAtt.AutoCreateAndBind)
            {
                IocManager.Instance.Logger<RabbitInstaller>().LogInformation(message: $"正在初始化：{consumerType.Name}");
                var rabbitManager = new RabbitManager(rabbitItemConfig: rabbitItemConfig);

                // 配置死信参数
                var arguments                                                                                                 = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(value: consumerAtt.DlxExchangeName)) arguments[key: "x-dead-letter-exchange"]  = consumerAtt.DlxExchangeName;
                if (!string.IsNullOrWhiteSpace(value: consumerAtt.DlxRoutingKey)) arguments[key: "x-dead-letter-routing-key"] = consumerAtt.DlxRoutingKey;
                if (consumerAtt.DlxTime > 0) arguments[key: "x-message-ttl"]                                                  = consumerAtt.DlxTime;
                rabbitManager.CreateExchange(exchangeName: consumerAtt.ExchangeName, exchangeType: consumerAtt.ExchangeType);

                // 创建队列并绑定到交换器
                rabbitManager.CreateQueueAndBind(queueName: consumerAtt.QueueName, exchangeName: consumerAtt.ExchangeName, routingKey: consumerAtt.RoutingKey, arguments: arguments);
            }

            // 注册消费端            
            iocManager.Logger<RabbitInstaller>().LogInformation(message: $"正在启动：{consumerType.Name} Rabbit消费");
            var consumerInstance = new RabbitConsumer(iocManager: iocManager, consumerType: consumerType, rabbitItemConfig: rabbitItemConfig, queueName: consumerAtt.QueueName, lastAckTimeoutRestart: consumerAtt.LastAckTimeoutRestart, prefetchCount: consumerAtt.PrefetchCountOrPullNums);
            consumerInstance.Start();

            return Task.FromResult(result: 0);
        }
    }
}