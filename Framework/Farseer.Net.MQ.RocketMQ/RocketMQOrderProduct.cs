using System;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.RocketMQ
{
    /// <summary>
    ///     生产消息
    /// </summary>
    internal class RocketMQOrderProduct : IRocketMQOrderProduct
    {
        private readonly ONSFactoryProperty _factoryInfo;
        private OrderProducer _producer;

        /// <summary>
        ///     生产消息
        /// </summary>
        /// <param name="factoryInfo">消息队列属性</param>
        public RocketMQOrderProduct(ONSFactoryProperty factoryInfo)
        {
            _factoryInfo = factoryInfo;
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        public void Start()
        {
            if (_producer == null)
            {
                // 获取实产者实例
                _producer = ONSFactory.getInstance().createOrderProducer(_factoryInfo);
                _producer.start();
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="shardingKey"></param>
        /// <param name="tag">消息标签</param>
        /// <param name="key">每条消息的唯一标识</param>
        public SendResultONS Send(string message, string shardingKey, string tag = null, string key = null)
        {
            if (string.IsNullOrWhiteSpace(tag)) tag = "";
            if (string.IsNullOrWhiteSpace(key)) key = Guid.NewGuid().ToString();
            var msg = new Message(_factoryInfo.getPublishTopics(), tag, key, message);
            return _producer.send(msg, shardingKey);
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            _producer?.shutdown();
            _producer = null;
        }
    }
}