using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.MQ.Rocket.Configuration;
using FS.MQ.Rocket.SDK.Http;
using FS.MQ.Rocket.SDK.Http.Model;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using Microsoft.Extensions.Logging;
using Action = FS.MQ.RocketMQ.SDK.Action;

namespace FS.MQ.Rocket
{
    internal class HttpRocketConsumer : IHttpRocketConsumer
    {
        /// <summary>
        ///     配置信息
        /// </summary>
        private readonly RocketItemConfig _config;

        private MQConsumer _consumer;
        private List<Task> taskConsumer;

        public HttpRocketConsumer(RocketItemConfig config)
        {
            _config      = config;
            taskConsumer = new List<Task>();
            if (_config.ConsumeThreadNums      == 0) _config.ConsumeThreadNums      = 16;
            if (_config.HttpConsumeWaitSeconds == 0) _config.HttpConsumeWaitSeconds = 3;
        }

        /// <summary>
        ///     消费订阅
        /// </summary>
        /// <param name="listen"> 消息监听处理 </param>
        /// <param name="subExpression"> 标签 </param>
        public void Start(HttpMessageListener listen, string tag = "*")
        {
            if (_consumer != null) throw new FarseerException(message: "当前已开启过该消费，无法重新开启，需先关闭上一次的消费（调用Close()）。");

            var _client = new MQClient(accessKeyId: _config.AccessKey, secretAccessKey: _config.SecretKey, regionEndpoint: _config.Server);
            _consumer = _client.GetConsumer(instanceId: _config.InstanceID, topicName: _config.Topic, consumer: _config.ConsumerID, messageTag: tag);

            taskConsumer.Add(item: Task.Factory.StartNew(action: () =>
            {
                // 在当前线程循环消费消息，建议是多开个几个线程并发消费消息
                while (true)
                    try
                    {
                        // 长轮询消费消息
                        // 长轮询表示如果topic没有消息则请求会在服务端挂住3s，3s内如果有消息可以消费则立即返回
                        Message message = null;

                        try
                        {
                            message = _consumer.ConsumeMessage(
                                                               batchSize: 1,                                     // 一次最多消费3条(最多可设置为16条)
                                                               waitSeconds: (uint)_config.HttpConsumeWaitSeconds // 长轮询时间3秒（最多可设置为30秒）
                                                              )
                                               ?.FirstOrDefault();
                        }
                        catch (Exception exp)
                        {
                            if (exp is MessageNotExistException) continue;

                            IocManager.Instance.Logger<HttpRocketConsumer>().LogError(exception: exp, message: "HttpRocketMQ拉取异常");
                            Thread.Sleep(millisecondsTimeout: 2000);
                        }

                        if (message == null) continue;

                        var handlers = new List<string>();
                        // 处理业务逻辑
                        var ack = listen.Consume(message: message);
                        try
                        {
                            // Message.nextConsumeTime前若不确认消息消费成功，则消息会重复消费
                            // 消息句柄有时间戳，同一条消息每次消费拿到的都不一样
                            if (ack == Action.CommitMessage)
                            {
                                handlers.Add(item: message.ReceiptHandle);
                                _consumer.AckMessage(receiptHandles: handlers);
                            }
                        }
                        catch (Exception exp2)
                        {
                            // 某些消息的句柄可能超时了会导致确认不成功
                            if (exp2 is AckMessageException ackExp)
                            {
                                foreach (var errorItem in ackExp.ErrorItems) IocManager.Instance.Logger<HttpRocketConsumer>().LogError(message: $"Ack message fail, RequestId:{ackExp.RequestId}\tErrorHandle:{errorItem.ReceiptHandle},ErrorCode:{errorItem.ErrorCode},ErrorMsg:{errorItem.ErrorMessage}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IocManager.Instance.Logger<HttpRocketConsumer>().LogError(exception: ex, message: "HttpRocketMQ异常");
                        Thread.Sleep(millisecondsTimeout: 2000);
                    }
            }, creationOptions: TaskCreationOptions.LongRunning));
        }

        /// <summary>
        ///     关闭订阅消费
        /// </summary>
        public void Close()
        {
            _consumer = null;
            if (taskConsumer != null)
            {
                foreach (var task in taskConsumer) task.Dispose();
            }

            taskConsumer.Clear();
            taskConsumer = null;
        }
    }
}