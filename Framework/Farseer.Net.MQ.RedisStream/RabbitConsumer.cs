using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS.Cache.Redis;
using FS.DI;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FS.MQ.RedisStream
{
    internal class RedisStreamConsumer
    {
        /// <summary>
        /// ioc
        /// </summary>
        private readonly IIocManager _iocManager;
        /// <summary>
        /// 消费监听
        /// </summary>
        private readonly string _consumerType;

        /// <summary>
        /// 线程数（默认8）
        /// </summary>
        private readonly int _consumeThreadNums;

        /// <summary>
        /// 队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        /// 依赖的Redis组件
        /// </summary>
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        /// 获取当前主机名称（容器名称）
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        /// 消费组
        /// </summary>
        private readonly string _groupName;

        /// <summary>
        /// 每次拉取数据的数量
        /// </summary>
        private readonly int _pullCount;

        /// <summary>
        /// 消费客户端
        /// </summary>
        /// <param name="iocManager">IOC</param>
        /// <param name="consumerType">消费端Type</param>
        /// <param name="redisCacheManager">依赖的Redis组件</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="lastAckTimeoutRestart">最后ACK多少秒超时则重连（默认5分钟）</param>
        /// <param name="consumeThreadNums">线程数（默认8）</param>
        /// <param name="groupName">消费组</param>
        /// <param name="pullCount">每次拉取数据的数量</param>
        public RedisStreamConsumer(IIocManager iocManager, string consumerType, IRedisCacheManager redisCacheManager, string queueName, int lastAckTimeoutRestart, int consumeThreadNums, string groupName, int pullCount)
        {
            this._iocManager        = iocManager;
            this._consumerType      = consumerType;
            this._redisCacheManager = redisCacheManager;
            this._redisCacheManager = redisCacheManager;
            this._consumeThreadNums = consumeThreadNums;
            this._groupName         = groupName;
            this._pullCount         = pullCount;
            this._queueName         = queueName;

            _hostName = Environment.GetEnvironmentVariable("HOSTNAME");
            _hostName = $"{_queueName}_{_groupName}";
        }

        /// <summary>
        /// 监控消费
        /// </summary>
        /// <param name="listener">消费事件</param>
        public void Start()
        {
            // 开启多线程消费
            for (var i = 0; i < _consumeThreadNums; i++)
            {
                ThreadPool.QueueUserWorkItem(async _ => { await ConnectAsync(); });
            }
        }

        /// <summary>
        /// 持续消费，并检查连接状态并自动恢复
        /// </summary>
        private async Task ConnectAsync()
        {
            while (true)
            {
                var streamEntries = await _redisCacheManager.Db.StreamReadGroupAsync(_queueName, _groupName, _hostName, count: _pullCount);
                if (streamEntries.Length == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();
                var consumeContext = new ConsumeContext
                {
                    MessageIds = streamEntries.Select(o => o.Id.ToString()).ToArray()
                };
                var messages = streamEntries.Select(o => o.Values[0].Value.ToString()).ToArray();

                var listener = _iocManager.Resolve<IListenerMessage>(_consumerType);
                var result   = false;
                try
                {
                    result = await listener.Consumer(messages, consumeContext);
                }
                catch (Exception e)
                {
                    // 消费失败后处理
                    _iocManager.Logger<RedisStreamConsumer>().LogError(e, listener.GetType().FullName);
                    try
                    {
                        result = await listener.FailureHandling(messages, consumeContext);
                    }
                    catch (Exception exception)
                    {
                        _iocManager.Logger<RedisStreamConsumer>().LogError(exception, "失败处理出现异常：" + listener.GetType().FullName);
                        result = false;
                    }
                }
                finally
                {
                    var ids = streamEntries.Select(o => o.Id).ToArray();
                    if (result)
                    {
                        await _redisCacheManager.Db.StreamAcknowledgeAsync(_queueName, _groupName, ids);
                    }
                    else
                        // 消费失败时，把pending队列的消息重新放回队列
                        _redisCacheManager.Db.StreamClaim(_queueName, _groupName, _groupName, sw.ElapsedMilliseconds, ids);
                }
            }
        }
    }
}