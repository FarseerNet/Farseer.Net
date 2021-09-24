using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     生产消息
    /// </summary>
    public interface IRocketOrderProduct
    {
        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message"> 消息主体 </param>
        /// <param name="shardingKey"> </param>
        /// <param name="tag"> 消息标签 </param>
        /// <param name="key"> 每条消息的唯一标识 </param>
        SendResultONS Send(string message, string shardingKey, string tag = null, string key = null);

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