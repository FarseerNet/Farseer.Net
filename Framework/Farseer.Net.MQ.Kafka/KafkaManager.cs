using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using FS.MQ.Kafka.Configuration;

namespace FS.MQ.Kafka
{
    /// <summary>
    ///     Kafka管理器接口
    /// </summary>
    public class KafkaManager : IKafkaManager
    {
        private readonly KafkaItemConfig _kakfaConfig;


        private readonly object                                                 lockObj   = new object();
        private readonly ConcurrentDictionary<string, Producer<string, string>> producers = new ConcurrentDictionary<string, Producer<string, string>>();


        /// <summary>
        ///     Kafka管理器接口
        /// </summary>
        public KafkaManager(KafkaItemConfig config)
        {
            _kakfaConfig = config;
        }

        /// <summary>
        ///     消息队列消费者
        /// </summary>
        /// <param name="topic"> </param>
        /// <param name="handle"> </param>
        public void Consume(string topic, Action<Message<string, string>, Consumer<string, string>> handle)
        {
            Consume(topic: topic, ct: new CancellationTokenSource().Token, handle: handle);
        }

        /// <summary>
        ///     消息队列消费者
        /// </summary>
        /// <param name="topic"> </param>
        /// <param name="ct"> 取消标志 </param>
        /// <param name="handle"> </param>
        public void Consume(string topic, CancellationToken ct, Action<Message<string, string>, Consumer<string, string>> handle)
        {
            var consumer = ConsumerCreate();
            consumer.OnMessage += (_, msg) =>
            {
                if (topic == msg.Topic) handle(arg1: msg, arg2: consumer);
            };
            consumer.Subscribe(topic: topic);
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                consumer.Poll(timeout: TimeSpan.FromMilliseconds(value: 100));
            }
        }

        /// <summary>
        ///     消息生产
        /// </summary>
        /// <param name="topic"> </param>
        /// <param name="message"> </param>
        public async Task<Message<string, string>> ProduceAsync(string topic, string message)
        {
            if (!producers.TryGetValue(key: topic, value: out var producer))
            {
                lock (lockObj)
                {
                    if (!producers.TryGetValue(key: topic, value: out producer))
                    {
                        var config = new Dictionary<string, object>
                        {
                            [key: "bootstrap.servers"] = _kakfaConfig.Server
                        };
                        producer = new Producer<string, string>(config: config, keySerializer: new StringSerializer(encoding: Encoding.UTF8),
                                                                valueSerializer: new StringSerializer(encoding: Encoding.UTF8));
                        producers.TryAdd(key: topic, value: producer);
                    }
                }
            }

            var result = await producer.ProduceAsync(topic: topic, key: null, val: message);
            return result;
        }


        /// <summary>
        ///     创建消费者对象
        /// </summary>
        private Consumer<string, string> ConsumerCreate()
        {
            var config = new Dictionary<string, object>
            {
                [key: "group.id"]                = _kakfaConfig.GroupID              ?? null,
                [key: "enable.auto.commit"]      = _kakfaConfig.EnabledAutoCommit    ?? false,
                [key: "statistics.interval.ms"]  = _kakfaConfig.StatisticsIntervalMS ?? 60_000,
                [key: "auto.commit.interval.ms"] = _kakfaConfig.AutoCommitIntervalMS ?? 60_000,
                [key: "bootstrap.servers"]       = _kakfaConfig.Server
            };
            var consumer = new Consumer<string, string>(config: config, keyDeserializer: new StringDeserializer(encoding: Encoding.UTF8),
                                                        valueDeserializer: new StringDeserializer(encoding: Encoding.UTF8));
            return consumer;
        }
    }
}