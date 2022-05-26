namespace FS.Core.Abstract.MQ.Queue
{
    /// <summary>
    ///     RocketMQ管理器
    /// </summary>
    public interface IQueueManager
    {
        /// <summary>
        ///     生产普通消息
        /// </summary>
        IQueueProduct Product { get; }
    }
}