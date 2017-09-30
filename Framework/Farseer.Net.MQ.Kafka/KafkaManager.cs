using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Farseer.Net.MQ.Kafka.Configuration;
using System.Threading;

namespace Farseer.Net.MQ.Kafka
{
    /// <summary>
    /// Kafka管理器接口
    /// </summary>
    public class KafkaManager : IKafkaManager
    {
        private readonly KafkaItemConfig _kakfaConfig;

        /// <summary>
        /// 消息队列消费者
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="handle"></param>
        public void Consume(string topic, Action<Message<string, string>, Consumer<string, string>> handle)
        {
            Consume(topic, new CancellationTokenSource().Token, handle);
        }

        /// <summary>
        /// 消息队列消费者
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="ct">取消标志</param>
        /// <param name="handle"></param>
        public void Consume(string topic, CancellationToken ct, Action<Message<string, string>, Consumer<string, string>> handle)
        {
            var consumer = ConsumerCreate();
            consumer.OnMessage += (_, msg) =>
            {
                if (topic == msg.Topic)
                    handle(msg, consumer);

            };
            consumer.Subscribe(topic);
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                consumer.Poll(TimeSpan.FromMilliseconds(100));
            }
        }



        /// <summary>
        /// 创建消费者对象
        /// </summary>
        private Consumer<string, string> ConsumerCreate()
        {
            var config = new Dictionary<string, object>()
            {
                ["group.id"] = _kakfaConfig.GroupID ?? null,
                ["enable.auto.commit"] = _kakfaConfig.EnabledAutoCommit ?? false,
                ["statistics.interval.ms"] = _kakfaConfig.StatisticsIntervalMS ?? 60_000,
                ["auto.commit.interval.ms"] = _kakfaConfig.AutoCommitIntervalMS ?? 60_000,
                ["bootstrap.servers"] = _kakfaConfig.Server,
            };
            var consumer = new Consumer<string, string>(config, new StringDeserializer(Encoding.UTF8),
                   new StringDeserializer(Encoding.UTF8));
            return consumer;
        }


        private object lockObj = new object();
        private ConcurrentDictionary<string, Producer<string, string>> producers = new ConcurrentDictionary<string, Producer<string, string>>();
        /// <summary>
        /// 消息生产
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        public async Task<Message<string, string>> ProduceAsync(string topic, string message)
        {
            if (!producers.TryGetValue(topic, out Producer<string, string> producer))
            {
                lock (lockObj)
                {
                    if (!producers.TryGetValue(topic, out producer))
                    {
                        var config = new Dictionary<string, object>()
                        {
                            ["bootstrap.servers"] = _kakfaConfig.Server,
                        };
                        producer = new Producer<string, string>(config, new StringSerializer(Encoding.UTF8),
                            new StringSerializer(Encoding.UTF8));
                        producers.TryAdd(topic, producer);
                    }
                }
            }
            var result = await producer.ProduceAsync(topic, null, message);
            return result;
        }


        /// <summary>
        /// Kafka管理器接口
        /// </summary>
        public KafkaManager(KafkaItemConfig config)
        {
            this._kakfaConfig = config;
        }
    }
}
