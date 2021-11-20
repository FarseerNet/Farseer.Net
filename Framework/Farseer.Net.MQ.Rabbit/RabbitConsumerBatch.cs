using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     批量拉取消息
    /// </summary>
    public class RabbitConsumerBatch
    {
        /// <summary>
        ///     批量拉取消息数量
        /// </summary>
        private readonly uint _batchPullMessageCount;

        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly RabbitConnect _connect;

        /// <summary>
        ///     消费监听
        /// </summary>
        private readonly string _consumerTypeName;

        /// <summary>
        ///     ioc
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        ///     创建连接会话对象
        /// </summary>
        private IModel _channel;

        /// <summary>
        /// 是否使用链路追踪
        /// </summary>
        private bool _useLinkTrack;

        /// <summary>
        ///     消费客户端
        /// </summary>
        /// <param name="iocManager"> IOC </param>
        /// <param name="rabbitItemConfig"> </param>
        /// <param name="queueName"> 队列名称 </param>
        /// <param name="batchPullMessageCount"> 批量拉取消息数量（默认10） </param>
        /// <param name="consumerType"> 消费监听 </param>
        public RabbitConsumerBatch(IIocManager iocManager, Type consumerType, RabbitItemConfig rabbitItemConfig, string queueName, uint batchPullMessageCount)
        {
            _iocManager            = iocManager;
            _connect               = new RabbitConnect(config: rabbitItemConfig);
            _batchPullMessageCount = batchPullMessageCount < 1 ? 10 : batchPullMessageCount;
            _queueName             = queueName;
            _consumerTypeName      = consumerType.FullName;
            if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);
            _useLinkTrack = _iocManager.IsRegistered<ILinkTrackQueue>();
        }

        /// <summary>
        ///     循环拉取
        /// </summary>
        /// <param name="waitSleep"> 拉到消息处理完后，休息多长时间，继续下一轮拉取 </param>
        /// <param name="autoAck"> 是否自动确认消息 </param>
        public async Task StartWhile(int waitSleep, bool autoAck = false)
        {
            // 采用轮询的方式持续拉取
            while (true)
            {
                var result = 0;
                try
                {
                    // 拉取消息
                    result = await Start(autoAck: autoAck);
                }
                catch (Exception e)
                {
                    _iocManager.Logger<RabbitConsumerBatch>().LogError(exception: e, message: $"{_consumerTypeName}消费异常：{e.Message}");
                }

                await Task.Delay(millisecondsDelay: result > 0 ? waitSleep : 500);
            }
        }

        /// <summary>
        ///     监控消费（只消费一次）
        /// </summary>
        /// <param name="autoAck"> 是否自动确认，默认false </param>
        public async Task<int> Start(bool autoAck = false)
        {
            Connect();
            var   lstResult         = new List<string>();
            var   lstBasicGetResult = new List<BasicGetResult>();
            ulong deliveryTag       = 0;

            var listener = _iocManager.Resolve<IListenerMessageBatch>(name: _consumerTypeName);

            var result = false;
            try
            {
                var queueDeclarePassive = _channel.QueueDeclarePassive(queue: _queueName);
                if (queueDeclarePassive.MessageCount == 0) return 0;
                // 并发拉取多条消息
                var forCount = queueDeclarePassive.MessageCount > _batchPullMessageCount ? _batchPullMessageCount : queueDeclarePassive.MessageCount;
                Parallel.For(fromInclusive: 0, toExclusive: forCount, parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, body: index =>
                {
                    // 拉取一条消息
                    var resp = _channel.BasicGet(queue: _queueName, autoAck: autoAck);
                    if (resp == null) return;

                    lstBasicGetResult.Add(item: resp);

                    // 将byte消息转成string类型
                    var message = Encoding.UTF8.GetString(bytes: resp.Body.ToArray());
                    lstResult.Add(item: message);
                });

                // 没有取到数据时，直接退出
                if (lstResult.Count == 0) return 0;

                deliveryTag = lstBasicGetResult.Max(selector: o => o.DeliveryTag);

                // 消费
                using (FsLinkTrack.TrackMqConsumer(endPort: _connect.Connection.Endpoint.ToString(), queueName: _queueName, method: "RabbitConsumerBatch"))
                {
                    result = await listener.Consumer(messages: lstResult, resp: lstBasicGetResult);
                }

                // 写入链路追踪
                if (_useLinkTrack) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RabbitConsumer>().LogError(exception: e, message: e.Message);
                result = false;

                // 消费失败后处理
                try
                {
                    using (FsLinkTrack.TrackMqConsumer(endPort: _connect.Connection.Endpoint.ToString(), queueName: _queueName, method: "RabbitConsumer"))
                    {
                        result = await listener.FailureHandling(messages: lstResult, resp: lstBasicGetResult);
                    }

                    // 写入链路追踪
                    if (_useLinkTrack) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
                }
                catch (Exception exception)
                {
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(exception: exception, message: "失败处理出现异常：" + listener.GetType().FullName);
                    result = false;
                }
            }
            finally
            {
                if (!autoAck && lstResult.Count > 0)
                {
                    if (result)
                        _channel.BasicAck(deliveryTag: deliveryTag, multiple: true);
                    else
                        _channel.BasicNack(deliveryTag: deliveryTag, multiple: true, requeue: true); // 批量拒绝，代替_channel.BasicReject
                }

                Close();
            }

            // 成功时才返回前端数量，失败时，可由前端来决定休眠时间
            return result ? lstResult.Count : 0;
        }

        /// <summary>
        ///     单次消费连接MQ
        /// </summary>
        private void Connect()
        {
            if (_connect.Connection == null || !_connect.Connection.IsOpen) _connect.Open();
            if (_channel            == null || _channel.IsClosed) _channel = _connect.Connection.CreateModel();
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public void Close()
        {
            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }
        }
    }
}