using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket.Remove
{
    /// <summary>
    ///     生产消息
    /// </summary>
    public interface IRocketProduct
    {
        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        SendResultONS Send(string message, string tag = null, string key = null);

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="deliver"> 延迟消费ms </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        SendResultONS Send(string message, long deliver, string tag = null, string key = null);

        /// <summary>
        ///     关闭生产者
        /// </summary>
        void Close();
    }
}