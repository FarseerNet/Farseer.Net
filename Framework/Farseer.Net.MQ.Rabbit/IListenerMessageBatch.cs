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
    /// Rabbit批量拉取消息
    /// </summary>
    public interface IListenerMessageBatch
    {
        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="resp"></param>
        /// <returns>当开启手动确认时，返回true时，才会进行ACK确认</returns>
        Task<bool> Consumer(List<string> messages, List<BasicGetResult> resp);

        /// <summary>
        /// 当异常时处理
        /// </summary>
        /// <returns>true：表示成功处理，移除消息。false：处理失败，如果是重试状态，则放回队列</returns>
        Task<bool> FailureHandling(List<string> messages, List<BasicGetResult> resp) => Task.FromResult(false);

        /// <summary>
        /// 初始化并自动消费
        /// </summary>
        Task Init(IIocManager iocManager, ConsumerAttribute consumerAtt, Type consumerType)
        {
            // 读取配置
            var rabbitItemConfig = RabbitConfigRoot.Get().Find(o => o.Name == consumerAtt.Name);
            if (rabbitItemConfig == null)
            {
                iocManager.Logger<IListenerMessageBatch>().LogWarning($"未找到：{consumerType.FullName}的配置项：{consumerAtt.Name}");
                return Task.FromResult(0);
            }

            // 启用启动绑定时，要创建交换器、队列，并绑定
            if (consumerAtt.AutoCreateAndBind)
            {
                iocManager.Logger<IListenerMessageBatch>().LogInformation($"正在初始化：{consumerType.Name}");
                var rabbitManager = new RabbitManager(rabbitItemConfig);

                // 配置死信参数
                rabbitManager.CreateExchange(consumerAtt.ExchangeName, consumerAtt.ExchangeType);
                // 创建队列并绑定到交换器
                rabbitManager.CreateQueueAndBind(consumerAtt.QueueName, consumerAtt.ExchangeName, consumerAtt.RoutingKey);
            }

            // 注册消费端            
            FarseerApplication.AddInitCallback(async () =>
            {
                iocManager.Logger<IListenerMessageBatch>().LogInformation($"正在启动：{consumerType.Name} Rabbit批量消费");
                await Task.Factory.StartNew(async () =>
                {
                    var consumerInstance = new RabbitConsumerBatch(iocManager, consumerType, rabbitItemConfig, consumerAtt.QueueName, consumerAtt.ThreadNumsOrPullNums);
                    await consumerInstance.StartWhile(consumerAtt.BatchPullSleepTime);
                }, TaskCreationOptions.LongRunning);
            });

            return Task.FromResult(0);
        }
    }
}