using System;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.RocketMQ.Http
{
    /// <summary>
    ///     ������Ϣ
    /// </summary>
    internal class RocketMQOrderProduct : IRocketMQOrderProduct
    {
        private readonly ONSFactoryProperty _factoryInfo;
        private OrderProducer _producer;

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        /// <param name="factoryInfo">��Ϣ��������</param>
        public RocketMQOrderProduct(ONSFactoryProperty factoryInfo)
        {
            _factoryInfo = factoryInfo;
        }

        /// <summary>
        ///     ����������Ϣ
        /// </summary>
        public void Start()
        {
            if (_producer == null)
            {
                // ��ȡʵ����ʵ��
                _producer = ONSFactory.getInstance().createOrderProducer(_factoryInfo);
                _producer.start();
            }
        }

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="shardingKey"></param>
        /// <param name="tag">��Ϣ��ǩ</param>
        /// <param name="key">ÿ����Ϣ��Ψһ��ʶ</param>
        public SendResultONS Send(string message, string shardingKey, string tag = null, string key = null)
        {
            if (string.IsNullOrWhiteSpace(tag)) tag = "";
            if (string.IsNullOrWhiteSpace(key)) key = Guid.NewGuid().ToString();
            var msg = new Message(_factoryInfo.getPublishTopics(), tag, key, message);
            return _producer.send(msg, shardingKey);
        }

        /// <summary>
        ///     �ر�������
        /// </summary>
        public void Close()
        {
            _producer?.shutdown();
            _producer = null;
        }
    }
}