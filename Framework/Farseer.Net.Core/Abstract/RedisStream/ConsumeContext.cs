using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;

namespace FS.Core.Abstract.RedisStream
{
    public class ConsumeContext
    {
        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string _queueName;

        public ConsumeContext(string queueName, IEnumerable<RedisStreamMessage> redisStreamMessages)
        {
            _queueName          = queueName;
            RedisStreamMessages = redisStreamMessages;
        }

        public IEnumerable<RedisStreamMessage> RedisStreamMessages { get; set; }

        /// <summary>
        /// 本次拉取的消息，最后一个ID
        /// </summary>
        public string LastId => RedisStreamMessages.LastOrDefault()?.Id;

        /// <summary>
        /// 单独提交ACK的MessageId
        /// </summary>
        public IEnumerable<string> AckMessageIds => RedisStreamMessages.Where(o => o.IsAck).Select(o => o.Id);
    }
}