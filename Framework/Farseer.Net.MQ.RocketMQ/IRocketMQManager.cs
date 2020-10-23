namespace FS.MQ.RocketMQ
{
    /// <summary>
    ///     RocketMQ管理器
    /// </summary>
    public interface IRocketMQManager
    {
        /// <summary>
        ///     生产普通消息
        /// </summary>
        IRocketMQProduct Product { get; }

        ///// <summary>
        ///// 生产顺序消息
        ///// </summary>
        //IRocketMQOrderProduct OrderProduct { get; }
        /// <summary>
        ///     消费普通消息
        /// </summary>
        IRocketMQConsumer Consumer { get; }

        /// <summary>
        ///     生产普通消息（基于HTTP）
        /// </summary>
        IHttpRocketMQProduct HttpProduct { get; }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        IHttpRocketMQConsumer HttpConsumer { get; }

        ///// <summary>
        ///// 消费顺序消息
        ///// </summary>
        //IRocketMQOrderConsumer OrderConsumer { get; }
    }
}