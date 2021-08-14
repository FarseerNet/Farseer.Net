using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace FS.MQ.Rabbit
{
    internal class RabbitConsumer
    {
        /// <summary>
        /// ioc
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        /// 消费监听
        /// </summary>
        private string _consumerType;

        /// <summary>
        /// 创建消息队列属性
        /// </summary>
        private readonly RabbitConnect _connect;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel _channel;

        /// <summary>
        /// 后台定时检查连接状态
        /// </summary>
        private Task _checkConnectStatsTask;

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
        /// <param name="connect"></param>
        /// <param name="queueName">队列名称</param>
        /// <param name="lastAckTimeoutRestart">最后ACK多少秒超时则重连（默认5分钟）</param>
        /// <param name="consumeThreadNums">线程数（默认8）</param>
        public RabbitConsumer(IIocManager iocManager, string consumerType, RabbitConnect connect, string queueName, int lastAckTimeoutRestart, int consumeThreadNums)
        {
            this._iocManager            = iocManager;
            this._consumerType          = consumerType;
            this._connect               = connect;
            this._lastAckTimeoutRestart = lastAckTimeoutRestart;
            this._consumeThreadNums     = consumeThreadNums;
            this._queueName             = queueName;
            this._lastAckAt             = DateTime.Now;
        }

        /// <summary>
        /// 监控消费
        /// </summary>
        /// <param name="listener">消费事件</param>
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
            _checkConnectStatsTask?.Dispose();
            _checkConnectStatsTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // 未打开、关闭状态、上一次ACK超时，则重启
                    if (_channel == null || _channel.IsClosed || (DateTime.Now - _lastAckAt).TotalSeconds >= _lastAckTimeoutRestart) ReStart();
                    Thread.Sleep(3000);
                }
            });
        }

        /// <summary>
        /// 监控消费（只消费一次）
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void StartSingle(IListenerMessageSingle listener, bool autoAck = false)
        {
            Connect();

            // 只获取一次
            var resp = _channel.BasicGet(_queueName, autoAck);

            var result  = false;
            var message = Encoding.UTF8.GetString(resp.Body.ToArray());
            try
            {
                using (FsLinkTrack.TrackMqConsumer(_queueName))
                {
                    result = listener.Consumer(message, resp);
                }

                // 写入链路追踪
                _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RabbitConsumer>().LogError(e, e.Message);
                // 消费失败后处理
                try
                {
                    using (FsLinkTrack.TrackMqConsumer(_queueName))
                    {
                        result = listener.FailureHandling(message, resp);
                    }

                    // 写入链路追踪
                    _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
                }
                catch (Exception exception)
                {
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(exception, "失败处理出现异常：" + listener.GetType().FullName);
                    result = false;
                }
            }
            finally
            {
                if (!autoAck)
                {
                    if (result) _channel.BasicAck(resp.DeliveryTag, false);
                    else _channel.BasicReject(resp.DeliveryTag, true);
                }

                Close();
            }
        }

        /// <summary>
        /// 单次消费连接MQ
        /// </summary>
        private void Connect()
        {
            if (_connect.Connection == null || !_connect.Connection.IsOpen) _connect.Open();
            if (_channel == null || _channel.IsClosed) _channel = _connect.Connection.CreateModel();
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
                var listener = _iocManager.Resolve<IListenerMessage>(_consumerType);
                var result   = false;
                var message  = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    using (FsLinkTrack.TrackMqConsumer(_queueName))
                    {
                        result = await listener.Consumer(message, model, ea);
                    }

                    // 写入链路追踪
                    _iocManager.Resolve<ILinkTrackQueue>().Enqueue();

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
                        using (FsLinkTrack.TrackMqConsumer(_queueName))
                        {
                            result = await listener.FailureHandling(message, model, ea);
                        }

                        // 写入链路追踪
                        _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
                    }
                    catch (Exception exception)
                    {
                        IocManager.Instance.Logger<RabbitConsumer>().LogError(exception, "失败处理出现异常：" + listener.GetType().FullName);
                        result = false;
                    }
                }
                finally
                {
                    if (!autoAck)
                    {
                        if (result) _channel.BasicAck(ea.DeliveryTag, false);
                        else _channel.BasicReject(ea.DeliveryTag, true);
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
            _checkConnectStatsTask?.Dispose();
            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }
        }
    }
}