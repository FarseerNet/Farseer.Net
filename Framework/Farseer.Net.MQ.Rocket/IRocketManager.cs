using FS.MQ.Rocket.Remove;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     RocketMQ管理器
    /// </summary>
    public interface IRocketManager
    {
        /// <summary>
        ///     生产普通消息
        /// </summary>
        IRocketProduct Product { get; }

        ///// <summary>
        ///// 生产顺序消息
        ///// </summary>
        //IRocketMQOrderProduct OrderProduct { get; }
        /// <summary>
        ///     消费普通消息
        /// </summary>
        IRocketConsumer Consumer { get; }

        /// <summary>
        ///     生产普通消息（基于HTTP）
        /// </summary>
        IHttpRocketProduct HttpProduct { get; }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        IHttpRocketConsumer HttpConsumer { get; }

        ///// <summary>
        ///// 消费顺序消息
        ///// </summary>
        //IRocketMQOrderConsumer OrderConsumer { get; }
    }
}