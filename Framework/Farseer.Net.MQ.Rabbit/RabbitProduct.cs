using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ;
using FS.Core.AOP.LinkTrack;
using FS.Core.LinkTrack;
using FS.DI;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    public class RabbitProduct : IRabbitProduct
    {
        private static readonly object objLock = new();

        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly RabbitConnect _connect;

        /// <summary>
        ///     配置信息
        /// </summary>
        private readonly ProductConfig _productConfig;

        private readonly ConcurrentQueue<IModel> Stacks = new();
        public           string                  QueueName => _productConfig.ExchangeName;

        public RabbitProduct(RabbitConnect connect, ProductConfig productConfig)
        {
            if (productConfig.MinFreeChannelPool == 0) productConfig.MinFreeChannelPool = 8;
            if (productConfig.MaxFreeChannelPool == 0) productConfig.MaxFreeChannelPool = 10;
            _connect       = connect;
            _productConfig = productConfig;

            _connect.Open();

            // 启动后，后台立即创建10个频道
            KeepFeeChannelCount();
        }

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        public bool Send(string message, Action<IBasicProperties> funcBasicProperties = null) => Send(message: message, routingKey: _productConfig.RoutingKey, exchange: _productConfig.ExchangeName, funcBasicProperties: funcBasicProperties);

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        public bool Send(IEnumerable<string> message, Action<IBasicProperties> funcBasicProperties = null) => Send(message: message, routingKey: _productConfig.RoutingKey, exchange: _productConfig.ExchangeName, funcBasicProperties: funcBasicProperties);

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        public bool Send(string message, string routingKey, Action<IBasicProperties> funcBasicProperties = null) => Send(message: message, routingKey: routingKey, exchange: _productConfig.ExchangeName, funcBasicProperties: funcBasicProperties);

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        public bool Send(IEnumerable<string> message, string routingKey, Action<IBasicProperties> funcBasicProperties = null) => Send(message: message, routingKey: routingKey, exchange: _productConfig.ExchangeName, funcBasicProperties: funcBasicProperties);

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="exchange"> 交换器名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        [TrackMqProduct(MqType.Rabbit)]
        internal bool Send(string message, string routingKey, string exchange, Action<IBasicProperties> funcBasicProperties = null)
        {
            IModel channel = null;
            try
            {
                channel = CreateChannel();

                var basicProperties = channel.CreateBasicProperties();
                // 默认设置为消息持久化
                if (funcBasicProperties != null)
                    funcBasicProperties(obj: basicProperties);
                else
                    basicProperties.DeliveryMode = 2;

                //消息内容
                var body = Encoding.UTF8.GetBytes(s: message);
                //发送消息
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: body);
                return !_productConfig.UseConfirmModel || channel.WaitForConfirms();
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RabbitProduct>().LogError(exception: e, message: e.ToString());
                return false;
            }
            finally
            {
                Close(channel: channel);
            }
        }

        /// <summary>
        ///     发送消息（批量）
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="routingKey"> 路由KEY名称 </param>
        /// <param name="exchange"> 交换器名称 </param>
        /// <param name="funcBasicProperties"> 属性 </param>
        [TrackMqProduct(MqType.Rabbit)]
        public bool Send(IEnumerable<string> message, string routingKey, string exchange, Action<IBasicProperties> funcBasicProperties = null)
        {
            IModel channel = null;
            try
            {
                channel = CreateChannel();

                var basicProperties = channel.CreateBasicProperties();
                // 默认设置为消息持久化
                if (funcBasicProperties != null)
                    funcBasicProperties(obj: basicProperties);
                else
                    basicProperties.DeliveryMode = 2;

                foreach (var msg in message)
                {
                    //消息内容
                    var body = Encoding.UTF8.GetBytes(s: msg);
                    //发送消息
                    channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: body);
                }

                return !_productConfig.UseConfirmModel || channel.WaitForConfirms();
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RabbitProduct>().LogError(exception: e, message: e.ToString());
                return false;
            }
            finally
            {
                Close(channel: channel);
            }
        }

        /// <summary>
        ///     保持最低空闲频道数量
        /// </summary>
        private void KeepFeeChannelCount()
        {
            Task.Factory.StartNew(action: () =>
            {
                while (true)
                {
                    // 当低于最低空闲的频道池时，快速创建到指定数量
                    while (Stacks.Count < _productConfig.MinFreeChannelPool)
                    {
                        var channel = _connect.Connection.CreateModel();
                        if (_productConfig.UseConfirmModel) channel.ConfirmSelect();
                        Stacks.Enqueue(item: channel);
                    }

                    // 当超出最大空闲频道池时，则释放到指定的最大空闲频道
                    while (Stacks.Count > _productConfig.MaxFreeChannelPool)
                    {
                        // 从池中取出频道
                        var tryPop = Stacks.TryDequeue(result: out var channel);

                        // 取出失败，说明没有可用频道，需要创建新的
                        if (tryPop && channel is
                            {
                                IsClosed: false
                            })
                        {
                            channel.Close();
                            channel.Dispose();
                        }
                    }

                    Thread.Sleep(timeout: TimeSpan.FromSeconds(value: 3));
                }
            });
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        private IModel CreateChannel()
        {
            // 如果连接断开，则要重连
            if (_connect.Connection == null || !_connect.Connection.IsOpen) _connect.Open();

            lock (objLock)
            {
                // 从池中取出频道
                var tryPop = Stacks.TryDequeue(result: out var channel);

                // 取出失败，说明没有可用频道，需要创建新的
                if (tryPop && channel is
                    {
                        IsClosed: false
                    })
                    return channel;

                channel = _connect.Connection.CreateModel();
                if (_productConfig.UseConfirmModel) channel.ConfirmSelect();
                return channel;
            }
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close(IModel channel)
        {
            Stacks.Enqueue(item: channel);
        }
    }
}