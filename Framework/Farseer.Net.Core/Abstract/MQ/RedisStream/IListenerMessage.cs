using System.Threading.Tasks;
using FS.Core.AOP.LinkTrack;
using FS.DI;

namespace FS.Core.Abstract.MQ.RedisStream
{
    /// <summary>
    ///     监听消费
    /// </summary>
    public interface IListenerMessage : ITransientDependency
    {
        /// <summary>
        ///     消费
        /// </summary>
        /// <returns> 当开启手动确认时，返回true时，才会进行ACK确认 </returns>
        /// <returns> true：下一次读取时，读取新的消息（不删除）。false：重新读取 </returns>
        [TrackMqConsumer(MqType.RedisStream)]
        Task<bool> Consumer(ConsumeContext context);

        /// <summary>
        ///     当异常时处理
        /// </summary>
        /// <returns> true：表示成功处理，移除消息。false：处理失败，如果是重试状态，则放回队列 </returns>
        [TrackMqConsumer(MqType.RedisStream)]
        Task<bool> FailureHandling(ConsumeContext context);
    }
}