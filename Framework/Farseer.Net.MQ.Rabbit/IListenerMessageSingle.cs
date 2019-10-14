using RabbitMQ.Client;

namespace FS.MQ.RabbitMQ
{
    /// <summary>
    /// Rabbit监听消费（单次）
    /// </summary>
    public interface IListenerMessageSingle
    {
        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="message"></param>
        /// <param name="resp"></param>
        /// <returns>当开启手动确认时，返回true时，才会进行ACK确认</returns>
        bool Consumer(string message, BasicGetResult resp);
    }
}