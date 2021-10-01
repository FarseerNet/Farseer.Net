using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.MQ.Rabbit.Attr;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace FS.MQ.Rabbit
{
    public class RabbitConsumer
    {
        /// <summary>
        ///     消费监听
        /// </summary>
        private readonly string _consumerTypeName;

        /// <summary>
        ///     线程数（默认8）
        /// </summary>
        private readonly uint _consumeThreadNums;

        /// <summary>
        ///     ioc
        /// </summary>
        private readonly IIocManager _iocManager;

        private readonly Type _consumerType;

        /// <summary>
        ///     最后ACK多少秒超时则重连（默认5分钟）
        /// </summary>
        private readonly int _lastAckTimeoutRestart;

        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly RabbitConnect _rabbitConnect;

        /// <summary>
        ///     是否自动ack
        /// </summary>
        private bool _autoAck;

        /// <summary>
        ///     创建连接会话对象
        /// </summary>
        private IModel _channel;

        /// <summary>
        ///     针对后台定时检查状态的取消令牌
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        ///     最后一次ACK确认时间
        /// </summary>
        private DateTime _lastAckAt;

        /// <summary>
        ///     消费客户端
        /// </summary>
        /// <param name="iocManager"> IOC </param>
        /// <param name="consumerType"> 消费端Type </param>
        /// <param name="rabbitItemConfig"> </param>
        /// <param name="queueName"> 队列名称 </param>
        /// <param name="lastAckTimeoutRestart"> 最后ACK多少秒超时则重连（默认5分钟） </param>
        /// <param name="consumeThreadNums"> 线程数（默认8） </param>
        public RabbitConsumer(IIocManager iocManager, Type consumerType, RabbitItemConfig rabbitItemConfig, string queueName, int lastAckTimeoutRestart, uint consumeThreadNums)
        {
            _iocManager            = iocManager;
            _consumerType          = consumerType;
            _consumerTypeName      = consumerType.FullName;
            _rabbitConnect         = new RabbitConnect(config: rabbitItemConfig);
            _lastAckTimeoutRestart = lastAckTimeoutRestart > 0 ? lastAckTimeoutRestart : 5 * 60;
            _consumeThreadNums     = consumeThreadNums     == 0 ? (uint)Environment.ProcessorCount : consumeThreadNums;
            _queueName             = queueName;
            _lastAckAt             = DateTime.Now;

            if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);
        }

        /// <summary>
        ///     监控消费
        /// </summary>
        /// <param name="autoAck"> 是否自动确认，默认false </param>
        public void Start(bool autoAck = false)
        {
            _autoAck = autoAck;
            Connect(autoAck: _autoAck);
            CheckStatsAndConnect();
        }

        /// <summary>
        ///     重启
        /// </summary>
        private void ReStart()
        {
            Close();
            Connect(autoAck: _autoAck);
        }

        /// <summary>
        ///     定时检查连接状态
        /// </summary>
        private void CheckStatsAndConnect()
        {
            // 检查连接状态
            _cts = new CancellationTokenSource();
            Task.Factory.StartNew(action: token =>
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
                            _iocManager.Logger<RabbitConsumer>().LogWarning(message: "发现Rabbit未连接，或已关闭，开始重新连接");
                            ReStart();
                        }
                        else if ((DateTime.Now - _lastAckAt).TotalSeconds >= _lastAckTimeoutRestart)
                        {
                            _iocManager.Logger<RabbitConsumer>().LogWarning(message: $"rabbit距上一次消费过去了{(DateTime.Now - _lastAckAt).TotalSeconds}秒后没有新的消息，尝试重新连接Rabbit。");
                            ReStart();
                        }

                        Thread.Sleep(millisecondsTimeout: 3000);
                    }
                }
                catch (Exception e)
                {
                    _iocManager.Logger<RabbitConsumer>().LogWarning(message: e.Message);
                }
            }, state: _cts.Token);
        }

        /// <summary>
        ///     单次消费连接MQ
        /// </summary>
        private void Connect()
        {
            if (_rabbitConnect.Connection == null || !_rabbitConnect.Connection.IsOpen) _rabbitConnect.Open();
            if (_channel                  == null || _channel.IsClosed) _channel = _rabbitConnect.Connection.CreateModel();
            _lastAckAt = DateTime.Now;
        }

        /// <summary>
        ///     持续消费，并检查连接状态并自动恢复
        /// </summary>
        private void Connect(bool autoAck = false)
        {
            Connect();

            _channel.BasicQos(prefetchSize: 0, prefetchCount: (ushort)_consumeThreadNums, global: false);
            var consumer = new EventingBasicConsumer(model: _channel);
            consumer.Received += async (model, ea) =>
            {
                ea.BasicProperties.Headers ??= new Dictionary<string, object>();
                ea.BasicProperties.Headers.Add(key: "QueueName", value: _queueName);

                var listener = _iocManager.Resolve<IListenerMessage>(name: _consumerTypeName);
                var result   = false;
                var message  = Encoding.UTF8.GetString(bytes: ea.Body.ToArray());
                try
                {
                    using (FsLinkTrack.TrackMqConsumer(endPort: _rabbitConnect.Connection.Endpoint.ToString(), queueName: _queueName, method: "RabbitConsumer"))
                    {
                        result = await listener.Consumer(message: message, sender: model, ea: ea);
                    }

                    // 写入链路追踪
                    if (_iocManager.IsRegistered<ILinkTrackQueue>()) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();

                    _lastAckAt = DateTime.Now;
                }
                catch (AlreadyClosedException e) // rabbit被关闭了，重新打开链接
                {
                    ReStart();
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(exception: e, message: listener.GetType().FullName);
                }
                catch (Exception e)
                {
                    // 消费失败后处理
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(exception: e, message: listener.GetType().FullName);
                    try
                    {
                        using (FsLinkTrack.TrackMqConsumer(endPort: _rabbitConnect.Connection.Endpoint.ToString(), queueName: _queueName, method: "RabbitConsumer"))
                        {
                            result = await listener.FailureHandling(message: message, sender: model, ea: ea);
                        }

                        // 写入链路追踪
                        if (_iocManager.IsRegistered<ILinkTrackQueue>()) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
                    }
                    catch (Exception exception)
                    {
                        IocManager.Instance.Logger<RabbitConsumer>().LogError(exception: exception, message: "失败处理出现异常：" + listener.GetType().FullName);
                        result = false;
                    }
                    finally
                    {
                        // 消息仍然失败，则对消息累加ErrorCount。
                        if (!result)
                        {
                            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
                            if (json.ContainsKey("ErrorCount"))
                            {
                                int.TryParse(json["ErrorCount"].ToString(), out var errorCount);
                                json["ErrorCount"] = errorCount + 1;
                                message            = JsonConvert.SerializeObject(json);
                            }

                            // 重新发消息
                            var consumerAtt   = _consumerType.GetCustomAttribute<ConsumerAttribute>();
                            var rabbitProduct = new RabbitProduct(_rabbitConnect, new ProductConfig() { UseConfirmModel = true, ExchangeType = consumerAtt.ExchangeType });
                            result = rabbitProduct.Send(message, consumerAtt.RoutingKey, consumerAtt.ExchangeName);
                        }
                    }
                }
                finally
                {
                    if (_channel is
                    {
                        IsOpen: true
                    } && !autoAck)
                    {
                        if (result)
                            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        else
                            _channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);
                    }
                }
            };
            // 消费者开启监听
            _channel.BasicConsume(queue: _queueName, autoAck: autoAck, consumer: consumer);
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