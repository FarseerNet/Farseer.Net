using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace FS.MQ.Kafka
{
    /// <summary>
    /// Kafka管理器接口
    /// </summary>
    public interface IKafkaManager
    {
        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="handle"></param>
        void Consume(string topic, Action<Message<string, string>, Consumer<string, string>> handle);

        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="ct"></param>
        /// <param name="handle"></param>
        void Consume(string topic, CancellationToken ct,Action<Message<string, string>, Consumer<string, string>> handle);

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        Task<Message<string, string>> ProduceAsync(string topic, string message);

    }
}
