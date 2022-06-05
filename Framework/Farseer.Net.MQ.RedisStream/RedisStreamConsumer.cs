using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Cache.Redis;
using FS.Core.Abstract.MQ.RedisStream;
using FS.Core.LinkTrack;
using FS.DI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace FS.MQ.RedisStream
{
    internal class RedisStreamConsumer
    {
        /// <summary>
        ///     消费监听
        /// </summary>
        private readonly string _consumerType;

        /// <summary>
        ///     线程数（默认8）
        /// </summary>
        private readonly int _consumeThreadNums;

        /// <summary>
        ///     消费组
        /// </summary>
        private readonly string _groupName;

        /// <summary>
        ///     获取当前主机名称（容器名称）
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        ///     ioc
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        ///     每次拉取数据的数量
        /// </summary>
        private readonly int _pullCount;

        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        ///     依赖的Redis组件
        /// </summary>
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        ///     消费客户端
        /// </summary>
        /// <param name="iocManager"> IOC </param>
        /// <param name="consumerType"> 消费端Type </param>
        /// <param name="redisCacheManager"> 依赖的Redis组件 </param>
        /// <param name="queueName"> 队列名称 </param>
        /// <param name="lastAckTimeoutRestart"> 最后ACK多少秒超时则重连（默认5分钟） </param>
        /// <param name="consumeThreadNums"> 线程数（默认8） </param>
        /// <param name="groupName"> 消费组 </param>
        /// <param name="pullCount"> 每次拉取数据的数量 </param>
        public RedisStreamConsumer(IIocManager iocManager, string consumerType, IRedisCacheManager redisCacheManager, string queueName, int lastAckTimeoutRestart, int consumeThreadNums, string groupName, int pullCount)
        {
            _iocManager        = iocManager;
            _consumerType      = consumerType;
            _redisCacheManager = redisCacheManager;
            _consumeThreadNums = consumeThreadNums == 0 ? Environment.ProcessorCount : consumeThreadNums;
            _groupName         = groupName;
            _pullCount         = pullCount;
            _queueName         = queueName;

            _hostName = $"{_queueName}_{_groupName}";
        }

        /// <summary>
        ///     监控消费
        /// </summary>
        public void Start()
        {
            // 开启多线程消费
            for (var i = 0; i < _consumeThreadNums; i++)
            // 如果消费组没有填写，则以广播模式消费
                if (string.IsNullOrWhiteSpace(value: _groupName))
                {
                    ThreadPool.QueueUserWorkItem(callBack: async _ =>
                    {
                        await ConnectAsync();
                    });
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(callBack: async _ =>
                    {
                        await ConnectGroupAsync();
                    });
                }
        }

        /// <summary>
        ///     持续消费
        /// </summary>
        private async Task ConnectGroupAsync()
        {
            while (true)
            {
                var            result          = false;
                var            consumerService = _iocManager.Resolve<IListenerMessage>(name: _consumerType);
                ConsumeContext consumeContext  = null;
                var            sw              = new Stopwatch();
                try
                {
                    var streamEntries = await _redisCacheManager.Db.StreamReadGroupAsync(key: _queueName, groupName: _groupName, consumerName: _hostName, count: _pullCount);
                    if (streamEntries.Length == 0)
                    {
                        await Task.Delay(millisecondsDelay: 300);
                        continue;
                    }
                    consumeContext = new ConsumeContext(_queueName, streamEntries.Select(o => new RedisStreamMessage
                    {
                        Id      = o.Id.ToString(),
                        Message = o.Values.FirstOrDefault().Value.ToString()
                    }));

                    sw.Restart();
                    result = await consumerService.Consumer(consumeContext);
                }
                catch (Exception e)
                {
                    // 消费失败后处理
                    _iocManager.Logger<RedisStreamConsumer>().LogError(exception: e, message: consumerService.GetType().FullName);
                    try
                    {
                        result = await consumerService.FailureHandling(consumeContext);
                    }
                    catch (Exception exception)
                    {
                        _iocManager.Logger<RedisStreamConsumer>().LogError(exception: exception, message: "失败处理出现异常：" + consumerService.GetType().FullName);
                        result = false;
                    }
                }
                finally
                {
                    IocManager.Instance.Release(consumerService);
                    var ids = consumeContext.RedisStreamMessages.Select(o => (RedisValue)o.Id).ToArray();
                    if (result)
                    {
                        await _redisCacheManager.Db.StreamAcknowledgeAsync(key: _queueName, groupName: _groupName, messageIds: ids);
                    }
                    else if (ids.Length > 0)
                    // 消费失败时，把pending队列的消息重新放回队列
                        _redisCacheManager.Db.StreamClaim(key: _queueName, consumerGroup: _groupName, claimingConsumer: _groupName, minIdleTimeInMs: sw.ElapsedMilliseconds, messageIds: ids);
                }
            }
        }

        /// <summary>
        ///     持续消费
        /// </summary>
        private async Task ConnectAsync()
        {
            var lastMessageId = "0";
            while (true)
            {
                var streamEntries = await _redisCacheManager.Db.StreamReadAsync(key: _queueName, position: lastMessageId, count: _pullCount);
                if (streamEntries.Length == 0)
                {
                    await Task.Delay(millisecondsDelay: 100);
                    continue;
                }
                var consumeContext = new ConsumeContext(_queueName, streamEntries.Select(o => new RedisStreamMessage
                {
                    Id      = o.Id.ToString(),
                    Message = o.Values.FirstOrDefault().Value.ToString()
                }));

                // 将拉取到的消息转成集合
                using var messages = consumeContext.RedisStreamMessages.Select(o => o.Message).ToPooledList();

                var sw = new Stopwatch();
                sw.Start();

                var listener = _iocManager.Resolve<IListenerMessage>(name: _consumerType);
                var result   = false;
                try
                {
                    result = await listener.Consumer(context: consumeContext);
                }
                catch (Exception e)
                {
                    // 消费失败后处理
                    _iocManager.Logger<RedisStreamConsumer>().LogError(exception: e, message: listener.GetType().FullName);
                    try
                    {
                        result = await listener.FailureHandling(context: consumeContext);
                    }
                    catch (Exception exception)
                    {
                        _iocManager.Logger<RedisStreamConsumer>().LogError(exception: exception, message: "失败处理出现异常：" + listener.GetType().FullName);
                        result = false;
                    }
                }
                finally
                {
                    if (result)
                    {
                        var ids = consumeContext.RedisStreamMessages.Select(o => (RedisValue)o.Id).ToArray();
                        await _redisCacheManager.Db.StreamDeleteAsync(key: _queueName, ids);
                        lastMessageId = consumeContext.LastId;
                    }

                    // 失败时，是否有单独需要ack的消息
                    if (!result && consumeContext.AckMessageIds.Any())
                    {
                        var lstAckId = consumeContext.AckMessageIds.Select(o => (RedisValue)o).ToArray();
                        await _redisCacheManager.Db.StreamDeleteAsync(key: _queueName, lstAckId);
                    }
                }
            }
        }
    }
}