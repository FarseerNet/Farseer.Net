namespace FS.MQ.RocketMQ
{
    /// <summary>
    ///     RocketMQ管理器
    /// </summary>
    public interface IRocketMQManager
    {
        /// <summary>
        ///     生产消息
        /// </summary>
        IRocketMQProduct Product { get; }

        ///// <summary>
        ///// 生产消息
        ///// </summary>
        //IRocketMQOrderProduct OrderProduct { get; }
        /// <summary>
        ///     订阅消费
        /// </summary>
        IRocketMQConsumer Consumer { get; }

        ///// <summary>
        ///// 消费
        ///// </summary>
        //IRocketMQOrderConsumer OrderConsumer { get; }
    }
}