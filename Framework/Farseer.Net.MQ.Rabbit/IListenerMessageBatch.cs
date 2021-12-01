using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.DI;
using FS.MQ.Rabbit.Attr;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     Rabbit批量拉取消息
    /// </summary>
    public interface IListenerMessageBatch
    {
        /// <summary>
        ///     消费
        /// </summary>
        /// <param name="messages"> </param>
        /// <param name="resp"> </param>
        /// <returns> 当开启手动确认时，返回true时，才会进行ACK确认 </returns>
        Task<bool> Consumer(List<string> messages, List<BasicGetResult> resp);

        /// <summary>
        ///     当异常时处理
        /// </summary>
        /// <returns> true：表示成功处理，移除消息。false：处理失败，如果是重试状态，则放回队列 </returns>
        Task<bool> FailureHandling(List<string> messages, List<BasicGetResult> resp) => Task.FromResult(result: false);

        /// <summary>
        ///     初始化并自动消费
        /// </summary>
        async Task Init(IIocManager iocManager, ConsumerAttribute consumerAtt, Type consumerType)
        {
            // 读取配置
            var rabbitItemConfig = RabbitConfigRoot.Get().Find(match: o => o.Server.Name == consumerAtt.Server);
            if (rabbitItemConfig == null)
            {
                iocManager.Logger<IListenerMessageBatch>().LogWarning(message: $"未找到：{consumerType.FullName}的配置项：{consumerAtt.Server}");
                return;
            }

            // 启用启动绑定时，要创建交换器、队列，并绑定
            if (consumerAtt.AutoCreateAndBind)
            {
                iocManager.Logger<IListenerMessageBatch>().LogInformation(message: $"正在初始化：{consumerType.Name}");
                var rabbitManager = new RabbitManager(rabbitItemConfig: rabbitItemConfig);

                // 配置死信参数
                rabbitManager.CreateExchange(exchangeName: consumerAtt.ExchangeName, exchangeType: consumerAtt.ExchangeType);
                // 创建队列并绑定到交换器
                rabbitManager.CreateQueueAndBind(queueName: consumerAtt.QueueName, exchangeName: consumerAtt.ExchangeName, routingKey: consumerAtt.RoutingKey);
            }

            // 注册消费端            
            iocManager.Logger<IListenerMessageBatch>().LogInformation(message: $"正在启动：{consumerType.Name} Rabbit批量消费");
            await Task.Factory.StartNew(function: async () =>
            {
                var consumerInstance = new RabbitConsumerBatch(iocManager: iocManager, consumerType: consumerType, rabbitItemConfig: rabbitItemConfig, queueName: consumerAtt.QueueName, batchPullMessageCount: consumerAtt.PrefetchCountOrPullNums);
                await consumerInstance.StartWhile(waitSleep: consumerAtt.BatchPullSleepTime);
            }, creationOptions: TaskCreationOptions.LongRunning);
        }
    }
}