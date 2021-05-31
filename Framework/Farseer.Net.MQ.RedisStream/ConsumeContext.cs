using System.Threading.Tasks;
using FS.Cache.Redis;
using StackExchange.Redis;

namespace FS.MQ.RedisStream
{
    public class ConsumeContext
    {
        /// <summary>
        /// 依赖的Redis组件
        /// </summary>
        private readonly IRedisCacheManager _redisCacheManager;

        /// <summary>
        /// 队列名称
        /// </summary>
        private readonly string _queueName;

        public ConsumeContext(IRedisCacheManager redisCacheManager, string queueName)
        {
            _redisCacheManager = redisCacheManager;
            _queueName         = queueName;
        }

        public string[] MessageIds { get; internal set; }

        /// <summary>
        /// 删除messageId
        /// </summary>
        public async Task Ack(StreamEntry message)
        {
            await _redisCacheManager.Db.StreamDeleteAsync(_queueName, new[] {message.Id});
        }
    }
}