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
            _factoryInfo = factoryInfo;
            _productConfig = productConfig;
            Connect();
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        private void Connect()
        {
            _con = _factoryInfo.CreateConnection();
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
        /// <param name="basicProperties">属性</param>
        public bool Send(string message, IBasicProperties basicProperties = null)
        {
            return Send(message, _productConfig.RoutingKey, _productConfig.ExchangeName, basicProperties);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="routingKey">路由KEY名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="basicProperties">属性</param>
        public bool Send(string message, string routingKey, string exchange = "", IBasicProperties basicProperties = null)
        {
            if (_channel == null || _channel.IsClosed) Connect();
            // 默认设置为消息持久化
            if (basicProperties == null)
            {
                basicProperties = _channel.CreateBasicProperties();
                basicProperties.DeliveryMode = 2;
            }
            
            //消息内容
            var body = Encoding.UTF8.GetBytes(message);
            //发送消息
            _channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: body);
            return !_productConfig.UseConfirmModel || _channel.WaitForConfirms();
        }
    }
}