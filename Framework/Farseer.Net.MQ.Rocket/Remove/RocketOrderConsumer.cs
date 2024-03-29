﻿// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-28 9:51
// ********************************************

using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket.Remove
{
    internal class RocketOrderConsumer : IRocketOrderConsumer
    {
        private readonly ONSFactoryProperty _factoryInfo;
        private          OrderConsumer      _consumer;

        public RocketOrderConsumer(ONSFactoryProperty factoryInfo)
        {
            _factoryInfo = factoryInfo;
        }

        /// <summary>
        ///     消费订阅
        /// </summary>
        /// <param name="listen"> 消息监听处理 </param>
        /// <param name="subExpression"> 标签 </param>
        public void Start(MessageOrderListener listen, string subExpression = "*")
        {
            _consumer = ONSFactory.getInstance().createOrderConsumer(factoryProperty: _factoryInfo);
            _consumer.subscribe(topic: _factoryInfo.getPublishTopics(), subExpression: subExpression, listener: listen);
            _consumer.start();
        }

        /// <summary>
        ///     关闭消费
        /// </summary>
        public void Close()
        {
            _consumer.shutdown();
        }
    }
}