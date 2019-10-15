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
        private readonly RabbitItemConfig _config;

        /// <summary>
        /// 创建连接对象
        /// </summary>
        private IConnection _con;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel _channel;

        public RabbitProduct(IConnectionFactory factoryInfo, RabbitItemConfig config)
        {
            _factoryInfo = factoryInfo;
            _config = config;
            Connect();
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        private void Connect()
        {
            _con = _factoryInfo.CreateConnection();
            _channel = _con.CreateModel();
            
            if (_config.UseConfirmModel) _channel.ConfirmSelect();
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
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="basicProperties">属性</param>
        public bool Send(string message, IBasicProperties basicProperties = null)
        {
            return Send(message, _config.QueueName, _config.ExchangeName, basicProperties);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="basicProperties">属性</param>
        public bool Send(string message, string queueName, string exchange = "", IBasicProperties basicProperties = null)
        {
            if (_channel == null) Connect();
            
            //消息内容
            var body = Encoding.UTF8.GetBytes(message);
            //发送消息
            _channel.BasicPublish(exchange: exchange, routingKey: queueName, basicProperties: null, body: body);
            return !_config.UseConfirmModel || _channel.WaitForConfirms();
        }
    }
}