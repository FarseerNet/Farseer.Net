using System.Text;
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
        private readonly RabbitItemConfig _config;

        /// <summary>
        /// 创建连接对象
        /// </summary>
        private IConnection _con;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel _channel;

        public RabbitConsumer(IConnectionFactory factoryInfo, RabbitItemConfig config)
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
        /// 监控消费
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void Start(IListenerMessage listener, bool autoAck = false)
        {
            if (_channel == null) Connect();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var result = listener.Consumer(Encoding.UTF8.GetString(ea.Body), model, ea);
                if (autoAck) return;
                if (result) _channel.BasicAck(ea.DeliveryTag, false);
                else _channel.BasicReject(ea.DeliveryTag, true);
            };

            // 消费者开启监听
            _channel.BasicConsume(queue: _config.QueueName, autoAck: autoAck, consumer: consumer);
        }

        /// <summary>
        /// 监控消费（只消费一次）
        /// </summary>
        /// <param name="listener">消费事件</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public void StartSignle(IListenerMessageSingle listener, bool autoAck = false)
        {
            if (_channel == null) Connect();
            // 只获取一次
            var resp = _channel.BasicGet(_config.QueueName, autoAck);

            var result = listener.Consumer(Encoding.UTF8.GetString(resp.Body), resp);

            if (autoAck) return;
            if (result) _channel.BasicAck(resp.DeliveryTag, false);
            else _channel.BasicReject(resp.DeliveryTag, true);
        }
    }
}