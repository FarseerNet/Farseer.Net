using System;
using System.Collections.Generic;
using System.Text;
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
        private List<IConnection> _con;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private List<IModel> _channel;

        public RabbitConsumer(IConnectionFactory factoryInfo, ConsumerConfig consumerConfig)
        {
            _factoryInfo = factoryInfo;
            _consumerConfig = consumerConfig;

            Connect();
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        private void Connect()
        {
            _con = new List<IConnection>();
            _channel = new List<IModel>();
            for (int i = 0; i < _consumerConfig.ConsumeThreadNums; i++)
            {
                var con = _factoryInfo.CreateConnection();
                _con.Add(con);
                _channel.Add(con.CreateModel());
            }
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            foreach (var model in _channel)
            {
                model.Close();
                model.Dispose();
            }

            _channel.Clear();
            _channel = null;

            foreach (var connection in _con)
            {
                connection.Close();
                connection.Dispose();
            }

            _con.Clear();
            _con = null;
        }

        /// <summary>
        /// 监控消费
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void Start(IListenerMessage listener, bool autoAck = false)
        {
            if (_channel?.Count == 0) Connect();
            for (int i = 0; i < _consumerConfig.ConsumeThreadNums; i++)
            {
                var chl = _channel[i];
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

        /// <summary>
        /// 监控消费（只消费一次）
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void StartSignle(IListenerMessageSingle listener, bool autoAck = false)
        {
            if (_channel?.Count == 0) Connect();

            for (int i = 0; i < _consumerConfig.ConsumeThreadNums; i++)
            {
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
    }
}