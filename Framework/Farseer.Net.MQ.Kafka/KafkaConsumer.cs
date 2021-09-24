// ********************************************
// 作者：洪根祥
// 时间：2017-07-06 17:34:50
// ********************************************

using System;
using Confluent.Kafka;

namespace FS.MQ.Kafka
{
    /// <summary>
    /// </summary>
    public class KafkaClient
    {
        /// <summary>
        /// </summary>
        internal Consumer<string, string> Consumer { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="topic"> </param>
        /// <param name="handle"> </param>
        public void Consume(string topic, Action<Message<string, string>> handle)
        {
            Consumer.OnMessage += (_, msg) =>
            {
                if (topic == msg.Topic) handle(obj: msg);
            };
            Consumer.Subscribe(topic: topic);
        }
    }
}