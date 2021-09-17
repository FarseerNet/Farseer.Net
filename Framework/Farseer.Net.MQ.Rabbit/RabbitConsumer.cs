using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace FS.MQ.Rabbit
{
    public class RabbitConsumer
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
        /// 创建消息队列属性
        /// </summary>
        private readonly RabbitConnect _rabbitConnect;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel _channel;

        /// <summary>
        /// 针对后台定时检查状态的取消令牌
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        /// 最后一次ACK确认时间
        /// </summary>
        private DateTime _lastAckAt;

        /// <summary>
        /// 是否自动ack
        /// </summary>
        private bool _autoAck;

        /// <summary>
        /// 最后ACK多少秒超时则重连（默认5分钟）
        /// </summary>
        private readonly int _lastAckTimeoutRestart;

        /// <summary>
        /// 线程数（默认8）
        /// </summary>
        private readonly int _consumeThreadNums;

        /// <summary>
        /// 队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        /// 消费客户端
        /// </summary>
        /// <param name="iocManager">IOC</param>
        /// <param name="consumerType">消费端Type</param>
        /// <param name="rabbitItemConfig"></param>
        /// <param name="queueName">队列名称</param>
        /// <param name="lastAckTimeoutRestart">最后ACK多少秒超时则重连（默认5分钟）</param>
        /// <param name="consumeThreadNums">线程数（默认8）</param>
        public RabbitConsumer(IIocManager iocManager, Type consumerType, RabbitItemConfig rabbitItemConfig, string queueName, int lastAckTimeoutRestart, int consumeThreadNums)
        {
            this._iocManager            = iocManager;
            this._consumerType          = consumerType.FullName;
            this._rabbitConnect         = new RabbitConnect(rabbitItemConfig);
            this._lastAckTimeoutRestart = lastAckTimeoutRestart > 0 ? lastAckTimeoutRestart : 5 * 60;
            this._consumeThreadNums     = consumeThreadNums == 0 ? Environment.ProcessorCount : consumeThreadNums;
            this._queueName             = queueName;
            this._lastAckAt             = DateTime.Now;

            if (!iocManager.IsRegistered(consumerType.FullName)) iocManager.Register(consumerType, consumerType.FullName, DependencyLifeStyle.Transient);
        }

        /// <summary>
        /// 监控消费
        /// </summary>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void Start(bool autoAck = false)
        {
            _autoAck = autoAck;
            Connect(_autoAck);
            CheckStatsAndConnect();
        }

        /// <summary>
        /// 重启
        /// </summary>
        private void ReStart()
        {
            Close();
            Connect(_autoAck);
        }

        /// <summary>
        /// 定时检查连接状态
        /// </summary>
        private void CheckStatsAndConnect()
        {
            // 检查连接状态
            _cts = new CancellationTokenSource();
            Task.Factory.StartNew(token =>
            {
                var cancellationToken = (CancellationToken)token;
                try
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        // 未打开、关闭状态、上一次ACK超时，则重启
                        if (_channel == null || _channel.IsClosed)
                        {
                            _iocManager.Logger<RabbitConsumer>().LogWarning($"发现Rabbit未连接，或已关闭，开始重新连接");
                            ReStart();
                        }
                        else if ((DateTime.Now - _lastAckAt).TotalSeconds >= _lastAckTimeoutRestart)
                        {
                            _iocManager.Logger<RabbitConsumer>().LogWarning($"rabbit距上一次消费过去了{(DateTime.Now - _lastAckAt).TotalSeconds}秒后没有新的消息，尝试重新连接Rabbit。");
                            ReStart();
                        }

                        Thread.Sleep(3000);
                    }
                }
                catch (Exception e)
                {
                    _iocManager.Logger<RabbitConsumer>().LogWarning(e.Message);
                }
            }, _cts.Token);
        }

        /// <summary>
        /// 单次消费连接MQ
        /// </summary>
        private void Connect()
        {
            if (_rabbitConnect.Connection == null || !_rabbitConnect.Connection.IsOpen) _rabbitConnect.Open();
            if (_channel == null || _channel.IsClosed) _channel = _rabbitConnect.Connection.CreateModel();
            _lastAckAt = DateTime.Now;
        }

        /// <summary>
        /// 持续消费，并检查连接状态并自动恢复
        /// </summary>
        private void Connect(bool autoAck = false)
        {
            Connect();

            _channel.BasicQos(0, (ushort)_consumeThreadNums, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                ea.BasicProperties.Headers ??= new Dictionary<string, object>();
                ea.BasicProperties.Headers.Add("QueueName", _queueName);

                var listener = _iocManager.Resolve<IListenerMessage>(_consumerType);
                var result   = false;
                var message  = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    using (FsLinkTrack.TrackMqConsumer(_rabbitConnect.Connection.Endpoint.ToString(), _queueName, "RabbitConsumer"))
                    {
                        result = await listener.Consumer(message, model, ea);
                    }

                    // 写入链路追踪
                    if (_iocManager.IsRegistered<ILinkTrackQueue>()) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();

                    _lastAckAt = DateTime.Now;
                }
                catch (AlreadyClosedException e) // rabbit被关闭了，重新打开链接
                {
                    ReStart();
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(e, listener.GetType().FullName);
                }
                catch (Exception e)
                {
                    // 消费失败后处理
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(e, listener.GetType().FullName);
                    try
                    {
                        using (FsLinkTrack.TrackMqConsumer(_rabbitConnect.Connection.Endpoint.ToString(), _queueName, "RabbitConsumer"))
                        {
                            result = await listener.FailureHandling(message, model, ea);
                        }

                        // 写入链路追踪
                        if (_iocManager.IsRegistered<ILinkTrackQueue>()) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
                    }
                    catch (Exception exception)
                    {
                        IocManager.Instance.Logger<RabbitConsumer>().LogError(exception, "失败处理出现异常：" + listener.GetType().FullName);
                        result = false;
                    }
                }
                finally
                {
                    if (_channel is { IsOpen: true } && !autoAck)
                    {
                        if (result) _channel.BasicAck(ea.DeliveryTag, false);
                        else _channel.BasicReject(ea.DeliveryTag, true);
                    }
                }
            };
            // 消费者开启监听
            _channel.BasicConsume(_queueName, autoAck, consumer);
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public void Close()
        {
            _cts?.Cancel();

            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }
        }
    }
}