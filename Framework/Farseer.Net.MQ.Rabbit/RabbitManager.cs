// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-23 22:50
// ********************************************

using System.Collections.Generic;
using FS.Configuration;
using FS.MQ.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace FS.MQ.RabbitMQ
{
    /// <summary>
    ///     Rabbit管理器
    /// </summary>
    public class RabbitManager : IRabbitManager
    {
        private static readonly object ObjLock = new object();

        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly IConnectionFactory _factoryInfo;

        /// <summary>
        ///     生产消息
        /// </summary>
        private IRabbitProduct _product;

        /// <summary>
        ///     消费消息
        /// </summary>
        private IRabbitConsumer _consumer;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly RabbitItemConfig _config;

        /// <summary> Rabbit管理器 </summary>
        public RabbitManager(RabbitItemConfig config)
        {
            _config = config;
            _factoryInfo = new ConnectionFactory //创建连接工厂对象
            {
                HostName = _config.Server, //IP地址
                Port = _config.Port, //端口号
                UserName = _config.UserName, //用户账号
                Password = _config.Password //用户密码
            };
        }

        /// <summary>
        ///     生产普通消息
        /// </summary>
        public IRabbitProduct Product
        {
            get
            {
                if (_product != null) return _product;
                lock (ObjLock)
                {
                    return _product ?? (_product = new RabbitProduct(_factoryInfo, _config));
                }
            }
        }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        public IRabbitConsumer Consumer
        {
            get
            {
                if (_consumer != null) return _consumer;
                lock (ObjLock)
                {
                    return _consumer ?? (_consumer = new RabbitConsumer(_factoryInfo, _config));
                }
            }
        }

        /// <summary>
        /// 创建队列（默认配置的队列名称）
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">是否持久化（默认true）</param>
        /// <param name="exclusive">是否排他队列（默认false）</param>
        /// <param name="autoDelete">是否自动删除（默认false）</param>
        /// <param name="arguments">队列参数</param>
        public void CreateQueue(bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            CreateQueue(_config.QueueName, durable, exclusive, autoDelete, arguments);
        }

        /// <summary>
        /// 创建队列
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">是否持久化（默认true）</param>
        /// <param name="exclusive">是否排他队列（默认false）</param>
        /// <param name="autoDelete">是否自动删除（默认false）</param>
        /// <param name="arguments">队列参数</param>
        public void CreateQueue(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            using (var conn = _factoryInfo.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    //声明一个队列
                    channel.QueueDeclare(
                        queue: queueName, //消息队列名称
                        durable: durable, //是否缓存
                        exclusive: exclusive, // 创建后删除
                        autoDelete: autoDelete,
                        arguments: arguments
                    );
                }
            }
        }

        /// <summary>
        /// 创建交换器，并绑定到队列
        /// </summary>
        /// <param name="exchangeName">交换器名称</param>
        /// <param name="exchangeType">交换器类型</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="durable">是否持久化（默认true）</param>
        /// <param name="autoDelete">是否自动删除（默认false）</param>
        /// <param name="arguments">参数</param>
        /// <param name="routingKey">路由标签</param>
        public void CreateExchange(string exchangeName, eumExchangeType exchangeType, string queueName, bool durable = true, bool autoDelete = false, IDictionary<string, object> arguments = null, string routingKey = "")
        {
            using (var conn = _factoryInfo.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, exchangeType.ToString(), durable, autoDelete, arguments); // 声明fanout交换器
                    channel.QueueBind(queueName, exchangeName, routingKey);
                }
            }
        }
    }
}