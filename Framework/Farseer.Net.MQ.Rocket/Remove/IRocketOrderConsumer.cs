﻿using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket.Remove
{
    /// <summary>
    ///     生产消息
    /// </summary>
    public interface IRocketOrderConsumer
    {
        /// <summary>
        ///     消费订阅
        /// </summary>
        /// <param name="listen"> 消息监听处理 </param>
        /// <param name="subExpression"> 标签 </param>
        void Start(MessageOrderListener listen, string subExpression = "*");

        /// <summary>
        ///     关闭订阅消费
        /// </summary>
        void Close();
    }
}