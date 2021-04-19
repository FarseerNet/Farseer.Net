using System;
using FS.MQ.Rocket.Configuration;
using FS.MQ.Rocket.SDK.Http;
using FS.MQ.Rocket.SDK.Http.Model;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     生产消息
    /// </summary>
    internal class HttpRocketProduct : IHttpRocketProduct
    {
        private MQProducer _producer;
        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly RocketItemConfig _config;
        
        /// <summary>
        ///     生产消息
        /// </summary>
        /// <param name="factoryInfo">消息队列属性</param>
        public HttpRocketProduct(RocketItemConfig config)
        {
            _config = config;
        }

        /// <summary>
        ///     开启生产消息
        /// </summary>
        public void Start()
        {
            if (_producer == null)
            {
                MQClient _client = new MQClient(_config.AccessKey, _config.SecretKey, _config.Server);
                _producer = _client.GetProducer(_config.InstanceID, _config.Topic);
            }
        }

        /// <summary>
        ///     关闭生产者
        /// </summary>
        public void Close()
        {
            _producer = null;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="tag">消息标签</param>
        /// <param name="key">每条消息的唯一标识</param>
        public TopicMessage Send(string message, string tag = null, string key = null)
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
        public TopicMessage Send(string message, long deliver, string tag = null, string key = null)
        {
            if (string.IsNullOrWhiteSpace(tag)) tag = "";
            if (string.IsNullOrWhiteSpace(key)) key = Guid.NewGuid().ToString();
            if (_config == null) throw new FarseerException("未开启Start方法，进行初始化");

            var sendMsg = new TopicMessage(message, tag) {MessageKey = key};
            // 设置属性
            //sendMsg.PutProperty("a", i.ToString());

            // 定时消息, 定时时间为10s后
            if (deliver > 0) sendMsg.StartDeliverTime = AliyunSDKUtils.GetNowTimeStamp() + deliver;
            return _producer.PublishMessage(sendMsg);
        }
    }
}