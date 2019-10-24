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
        private IConnection[] _con;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel[] _channel;

        private bool _isClose;

        /// <summary>
        /// 后台定时检查连接状态
        /// </summary>
        private Task _checkStatsAndConnectTask;

        public RabbitConsumer(IConnectionFactory factoryInfo, ConsumerConfig consumerConfig)
        {
            _factoryInfo = factoryInfo;
            _consumerConfig = consumerConfig;

            _con = new IConnection[_consumerConfig.ConsumeThreadNums];
            _channel = new IModel[_consumerConfig.ConsumeThreadNums];
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            _isClose = true;
            _checkStatsAndConnectTask?.Dispose();
            for (var index = 0; index < _channel.Length; index++)
            {
                _channel[index].Close();
                _channel[index].Dispose();
                _channel[index] = null;
            }

            for (var index = 0; index < _con.Length; index++)
            {
                _con[index].Close();
                _con[index].Dispose();
                _con[index] = null;
            }
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

            for (int i = 0; i < _consumerConfig.ConsumeThreadNums; i++)
            {
                if (!(_con[i]?.IsOpen).GetValueOrDefault() || (_channel[i]?.IsClosed).GetValueOrDefault())
                {
                    _con[i] = _factoryInfo.CreateConnection();
                    _channel[i] = _con[i].CreateModel();
                }

                // 只获取一次
                var chl = _channel[i];
                var resp = chl.BasicGet(_consumerConfig.QueueName, autoAck);

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
                        if (result) chl.BasicAck(resp.DeliveryTag, false);
                        else chl.BasicReject(resp.DeliveryTag, true);
                    }
                }
            }
        }

        /// <summary>
        /// 检查连接状态并自动恢复
        /// </summary>
        private void Connect(IListenerMessage listener, bool autoAck = false)
        {
            _isClose = false;
            for (int i = 0; i < _con.Length; i++)
            {
                if (!(_con[i]?.IsOpen).GetValueOrDefault() || (_channel[i]?.IsClosed).GetValueOrDefault())
                {
                    var con = _factoryInfo.CreateConnection();
                    var chl = con.CreateModel();

                    _con[i] = con;
                    _channel[i] = chl;
                    var consumer = new EventingBasicConsumer(chl);
                    consumer.Received += (model, ea) =>
                    {
                        var result = false;
                        try
                        {
                            result = listener.Consumer(Encoding.UTF8.GetString(ea.Body), model, ea);
                        }
                        catch (Exception e)
                        {
                            IocManager.Instance.Logger.Error(e.Message);
                        }
                        finally
                        {
                            if (!autoAck)
                            {
                                if (result) chl.BasicAck(ea.DeliveryTag, false);
                                else chl.BasicReject(ea.DeliveryTag, true);
                            }
                        }
                    };
                    // 消费者开启监听
                    chl.BasicConsume(queue: _consumerConfig.QueueName, autoAck: autoAck, consumer: consumer);
                }
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