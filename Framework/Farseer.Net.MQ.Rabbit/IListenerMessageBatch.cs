using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    /// <summary>
    /// Rabbit批量拉取消息
    /// </summary>
    public interface IListenerMessageBatch
    {
        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="message"></param>
        /// <param name="resp"></param>
        /// <returns>当开启手动确认时，返回true时，才会进行ACK确认</returns>
        Task<bool> Consumer(List<string> message, List<BasicGetResult> resp);

        /// <summary>
        /// 当异常时处理
        /// </summary>
        /// <returns>true：表示成功处理，移除消息。false：处理失败，如果是重试状态，则放回队列</returns>
        Task<bool> FailureHandling(List<string> message, List<BasicGetResult> resp);
    }
}