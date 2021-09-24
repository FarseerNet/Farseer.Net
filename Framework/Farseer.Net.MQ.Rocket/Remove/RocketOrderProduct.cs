using System;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket.Remove
{
    /// <summary>
    ///     生产消息
    /// </summary>
    internal class RocketOrderProduct : IRocketOrderProduct
    {
        private readonly ONSFactoryProperty _factoryInfo;
        private          OrderProducer      _producer;

        /// <summary>
        ///     生产消息
        /// </summary>
        /// <param name="factoryInfo"> 消息队列属性 </param>
        public RocketOrderProduct(ONSFactoryProperty factoryInfo)
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
                _producer = ONSFactory.getInstance().createOrderProducer(factoryProperty: _factoryInfo);
                _producer.start();
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="shardingKey"> </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        public SendResultONS Send(string message, string shardingKey, string tag = null, string key = null)
        {
            if (string.IsNullOrWhiteSpace(value: tag)) tag = "";
            if (string.IsNullOrWhiteSpace(value: key)) key = Guid.NewGuid().ToString();
            var msg                                        = new Message(topic: _factoryInfo.getPublishTopics(), tags: tag, keys: key, body: message);
            return _producer.send(msg: msg, shardingKey: shardingKey);
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