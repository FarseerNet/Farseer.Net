using System;
using System.Collections.Generic;
using System.Text;
using FS.MQ.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace FS.MQ.RabbitMQ
{
    public class RabbitProduct : IRabbitProduct
    {
        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly IConnectionFactory _factoryInfo;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly ProductConfig _productConfig;

        /// <summary>
        /// 创建连接对象
        /// </summary>
        private IConnection _con;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel _channel;

        public RabbitProduct(IConnectionFactory factoryInfo, ProductConfig productConfig)
        {
            _factoryInfo   = factoryInfo;
            _productConfig = productConfig;
            Connect();
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        private void Connect()
        {
            _con     = _factoryInfo.CreateConnection();
            _channel = _con.CreateModel();

            if (_productConfig.UseConfirmModel) _channel.ConfirmSelect();
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            _channel.Close();
            _channel.Dispose();
            _channel = null;


            _con.Close();
            _con.Dispose();
            _con = null;
        }

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="funcBasicProperties">属性</param>
        public bool Send(string message, Action<IBasicProperties> funcBasicProperties = null)
        {
            return Send(message, _productConfig.RoutingKey, _productConfig.ExchangeName, funcBasicProperties);
        }

        /// <summary>
        ///     发送消息（Routingkey默认配置中的RoutingKey；ExchangeName默认配置中的ExchangeName）
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="funcBasicProperties">属性</param>
        public bool Send(IEnumerable<string> message, Action<IBasicProperties> funcBasicProperties = null)
        {
            return Send(message, _productConfig.RoutingKey, _productConfig.ExchangeName, funcBasicProperties);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="routingKey">路由KEY名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="funcBasicProperties">属性</param>
        public bool Send(string message, string routingKey, string exchange = "", Action<IBasicProperties> funcBasicProperties = null)
        {
            if (!(_con?.IsOpen).GetValueOrDefault() || (_channel?.IsClosed).GetValueOrDefault()) Connect();

            var basicProperties = _channel.CreateBasicProperties();
            // 默认设置为消息持久化
            if (funcBasicProperties != null) funcBasicProperties(basicProperties);
            else basicProperties.DeliveryMode = 2;

            //消息内容
            var body = Encoding.UTF8.GetBytes(message);
            //发送消息
            _channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: body);
            return !_productConfig.UseConfirmModel || _channel.WaitForConfirms();
        }

        /// <summary>
        ///     发送消息（批量）
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="routingKey">路由KEY名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="funcBasicProperties">属性</param>
        public bool Send(IEnumerable<string> message, string routingKey, string exchange = "", Action<IBasicProperties> funcBasicProperties = null)
        {
            IConnection con     = null;
            IModel      channel = null;
            try
            {
                con     = _factoryInfo.CreateConnection();
                channel = _con.CreateModel();

                if (_productConfig.UseConfirmModel) channel.ConfirmSelect();

                var basicProperties = _channel.CreateBasicProperties();
                // 默认设置为消息持久化
                if (funcBasicProperties != null) funcBasicProperties(basicProperties);
                else basicProperties.DeliveryMode = 2;

                foreach (var msg in message)
                {
                    //消息内容
                    var body = Encoding.UTF8.GetBytes(msg);
                    //发送消息
                    channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: body);
                }

                return !_productConfig.UseConfirmModel || _channel.WaitForConfirms();
            }
            finally
            {
                channel?.Close();
                channel?.Dispose();

                con?.Close();
                con?.Dispose();
            }
        }
    }
}