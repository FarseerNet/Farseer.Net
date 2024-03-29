﻿using FS.MQ.Rocket.SDK.Http.Model;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     生产消息
    /// </summary>
    public interface IHttpRocketProduct
    {
        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        TopicMessage Send(string message, string tag = null, string key = null);

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="deliver"> 延迟消费ms </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        TopicMessage Send(string message, long deliver, string tag = null, string key = null);

        /// <summary>
        ///     关闭生产者
        /// </summary>
        void Close();

        /// <summary>
        ///     开启生产消息
        /// </summary>
        void Start();
    }
}