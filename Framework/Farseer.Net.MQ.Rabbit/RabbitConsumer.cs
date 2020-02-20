using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.MQ.RabbitMQ.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

        private bool _isClose;

        /// <summary>
        /// 后台定时检查连接状态
        /// </summary>
        private Task _checkStatsAndConnectTask;

        public RabbitConsumer(IConnectionFactory factoryInfo, ConsumerConfig consumerConfig)
        {
            _factoryInfo    = factoryInfo;
            _consumerConfig = consumerConfig;
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            _isClose = true;
            _checkStatsAndConnectTask?.Dispose();
            _channel.Close();
            _channel.Dispose();
            _channel = null;

            _con.Close();
            _con.Dispose();
            _con = null;
        }

        /// <summary>
        /// 监控消费
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void Start(IListenerMessage listener, bool autoAck = false)
        {
            _isClose = false;
            CheckStatsAndConnect(listener, autoAck);
        }

        /// <summary>
        /// 监控消费（只消费一次）
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void StartSignle(IListenerMessageSingle listener, bool autoAck = false)
        {
            _isClose = false;

            Connect();

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
            }
        }

        private void Connect()
        {
            if (!(_con?.IsOpen).GetValueOrDefault() || (_channel?.IsClosed).GetValueOrDefault())
            {
                _con     = _factoryInfo.CreateConnection();
                _channel = _con.CreateModel();
            }
        }

        /// <summary>
        /// 检查连接状态并自动恢复
        /// </summary>
        private void Connect(IListenerMessage listener, bool autoAck = false)
        {
            _isClose = false;
            if (!(_con?.IsOpen).GetValueOrDefault() || (_channel?.IsClosed).GetValueOrDefault())
            {
                _con     = _factoryInfo.CreateConnection();
                _channel = _con.CreateModel();
                _channel.BasicQos(0, (ushort) _consumerConfig.ConsumeThreadNums, false);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var result = false;
                    try
                    {
                        result = listener.Consumer(Encoding.UTF8.GetString(ea.Body), model, ea);
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
        /// 定时检查连接状态
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="autoAck"></param>
        private void CheckStatsAndConnect(IListenerMessage listener, bool autoAck = false)
        {
            _checkStatsAndConnectTask?.Dispose();
            _checkStatsAndConnectTask = Task.Factory.StartNew(() =>
            {
                while (!_isClose)
                {
                    Connect(listener, autoAck);
                    Thread.Sleep(1000);
                }
            });
        }
    }
}