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
        /// <summary>
        ///     配置信息
        /// </summary>
        private readonly RocketItemConfig _config;

        private MQProducer _producer;

        /// <summary>
        ///     生产消息
        /// </summary>
        /// <param name="factoryInfo"> 消息队列属性 </param>
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
                var _client = new MQClient(accessKeyId: _config.AccessKey, secretAccessKey: _config.SecretKey, regionEndpoint: _config.Server);
                _producer = _client.GetProducer(instanceId: _config.InstanceID, topicName: _config.Topic);
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
        /// <param name="message"> 消息主体 </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        public TopicMessage Send(string message, string tag = null, string key = null) => Send(message: message, deliver: 0, tag: tag, key: key);

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="deliver"> 延迟消费ms </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        public TopicMessage Send(string message, long deliver, string tag = null, string key = null)
        {
            if (string.IsNullOrWhiteSpace(value: tag)) tag = "";
            if (string.IsNullOrWhiteSpace(value: key)) key = Guid.NewGuid().ToString();
            if (_config == null) throw new FarseerException(message: "未开启Start方法，进行初始化");

            var sendMsg = new TopicMessage(body: message, messageTag: tag) { MessageKey = key };
            // 设置属性
            //sendMsg.PutProperty("a", i.ToString());

            // 定时消息, 定时时间为10s后
            if (deliver > 0) sendMsg.StartDeliverTime = AliyunSDKUtils.GetNowTimeStamp() + deliver;
            return _producer.PublishMessage(topicMessage: sendMsg);
        }
    }
}