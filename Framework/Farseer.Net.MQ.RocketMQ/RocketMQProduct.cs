using System;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.RocketMQ
{
    /// <summary>
    ///     生产消息
    /// </summary>
    internal class RocketMQProduct : IRocketMQProduct
    {
        private readonly ONSFactoryProperty _factoryInfo;
        private Producer _producer;

        /// <summary>
        ///     生产消息
        /// </summary>
        /// <param name="factoryInfo">消息队列属性</param>
        public RocketMQProduct(ONSFactoryProperty factoryInfo)
        {
            _factoryInfo = factoryInfo;
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        private void Connect()
        {
            if (_producer == null)
            {
                // 获取实产者实例
                _producer = ONSFactory.getInstance().createProducer(_factoryInfo);
                _producer.start();
            }
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            _producer?.shutdown();
            _producer = null;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="tag">消息标签</param>
        /// <param name="key">每条消息的唯一标识</param>
        public SendResultONS Send(string message, string tag = null, string key = null)
        {
            return Send(message, 0, tag, key);
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="deliver">延迟消费ms</param>
        /// <param name="tag">消息标签</param>
        /// <param name="key">每条消息的唯一标识</param>
        public SendResultONS Send(string message, long deliver, string tag = null, string key = null)
        {
            if (_producer == null) Connect();
            if (string.IsNullOrWhiteSpace(tag)) tag = "";
            if (string.IsNullOrWhiteSpace(key)) key = Guid.NewGuid().ToString();
            var msg = new Message(_factoryInfo.getPublishTopics(), tag, key, message);
            if (deliver > 0) msg.setStartDeliverTime(deliver);
            return _producer.send(msg);
        }
    }
}