using System;
using Farseer.Net.MQ.RocketMQ.SDK;

namespace Farseer.Net.MQ.RocketMQ
{
    /// <summary>
    ///     ������Ϣ
    /// </summary>
    internal class RocketMQProduct : IRocketMQProduct
    {
        private readonly ONSFactoryProperty _factoryInfo;
        private Producer _producer;

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        /// <param name="factoryInfo">��Ϣ��������</param>
        public RocketMQProduct(ONSFactoryProperty factoryInfo) { _factoryInfo = factoryInfo; }

        /// <summary>
        ///     ����������Ϣ
        /// </summary>
        public void Start()
        {
            if (_producer == null)
            {
                // ��ȡʵ����ʵ��
                _producer = ONSFactory.getInstance().createProducer(_factoryInfo);
                _producer.start();
            }
        }

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="tag">��Ϣ��ǩ</param>
        /// <param name="key">ÿ����Ϣ��Ψһ��ʶ</param>
        public SendResultONS Send(string message, string tag = null, string key = null) => Send(message, 0, tag, key);

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="deliver">�ӳ�����ms</param>
        /// <param name="tag">��Ϣ��ǩ</param>
        /// <param name="key">ÿ����Ϣ��Ψһ��ʶ</param>
        public SendResultONS Send(string message, long deliver, string tag = null, string key = null)
        {
            if (string.IsNullOrWhiteSpace(tag)) tag = "";
            if (string.IsNullOrWhiteSpace(key)) key = Guid.NewGuid().ToString();
            var msg = new Message(_factoryInfo.getPublishTopics(), tag, key, message);
            if (deliver > 0) msg.setStartDeliverTime(deliver);
            return _producer.send(msg);
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