using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.MQ.RabbitMQ.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace FS.MQ.RabbitMQ
{
    public class RabbitConsumer : IRabbitConsumer
    {
        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly IConnectionFactory _factoryInfo;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly ConsumerConfig _consumerConfig;

        /// <summary>
        /// 创建连接对象
        /// </summary>
        private IConnection _con;

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

        private IListenerMessage _listener;
        private bool             _autoAck;

        public RabbitConsumer(IConnectionFactory factoryInfo, ConsumerConfig consumerConfig)
        {
            _factoryInfo    = factoryInfo;
            _consumerConfig = consumerConfig;
            if (consumerConfig.LastAckTimeoutRestart == 0) consumerConfig.LastAckTimeoutRestart = 5 * 60;
        }

        /// <summary>
        /// 监控消费
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void Start(IListenerMessage listener, bool autoAck = false)
        {
            _listener = listener;
            _autoAck  = autoAck;
            Connect(_listener, _autoAck);
            CheckStatsAndConnect();
        }

        /// <summary>
        /// 重启
        /// </summary>
        private void ReStart()
        {
            Close();
            Connect(_listener, _autoAck);
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
                    if (!(_con?.IsOpen).GetValueOrDefault() || (_channel?.IsClosed).GetValueOrDefault() || (DateTime.Now - _lastAckAt).TotalSeconds >= _consumerConfig.LastAckTimeoutRestart) ReStart();
                    Thread.Sleep(3000);
                }
            });
        }

        /// <summary>
        /// 监控消费（只消费一次）
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void StartSignle(IListenerMessageSingle listener, bool autoAck = false)
        {
            Connect(listener.GetType().FullName);

            // 只获取一次
            var resp = _channel.BasicGet(_consumerConfig.QueueName, autoAck);

            var result = false;
            try
            {
                result = listener.Consumer(Encoding.UTF8.GetString(resp.Body), resp);
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger.Error(e.Message);
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
        /// <param name="clientProvidedName"> </param>
        private void Connect(string clientProvidedName)
        {
            if (!(_con?.IsOpen).GetValueOrDefault() || (_channel?.IsClosed).GetValueOrDefault())
            {
                _con     = _factoryInfo.CreateConnection(clientProvidedName);
                _channel = _con.CreateModel();
            }
        }

        /// <summary>
        /// 检查连接状态并自动恢复
        /// </summary>
        private void Connect(IListenerMessage listener, bool autoAck = false)
        {
            if (!(_con?.IsOpen).GetValueOrDefault() || (_channel?.IsClosed).GetValueOrDefault())
            {
                _con     = _factoryInfo.CreateConnection(listener.GetType().FullName);
                _channel = _con.CreateModel();
                _channel.BasicQos(0, (ushort) _consumerConfig.ConsumeThreadNums, false);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var result = false;
                    try
                    {
                        result     = listener.Consumer(Encoding.UTF8.GetString(ea.Body), model, ea);
                        _lastAckAt = DateTime.Now;
                    }
                    catch (AlreadyClosedException e) // rabbit被关闭了，重新打开链接
                    {
                        ReStart();
                        IocManager.Instance.Logger.Error(listener.GetType().FullName, e);
                    }
                    catch (Exception e)
                    {
                        IocManager.Instance.Logger.Error(listener.GetType().FullName, e);
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
                _channel.BasicConsume(queue: _consumerConfig.QueueName, autoAck: autoAck, consumer: consumer);
            }
        }

        /// <summary>
        ///     关闭生产者
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

            if (_con != null)
            {
                _con.Close();
                _con.Dispose();
                _con = null;
            }
        }
    }
}