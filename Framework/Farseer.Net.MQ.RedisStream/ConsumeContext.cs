using System.Threading.Tasks;
using FS.Cache.Redis;
using StackExchange.Redis;

namespace FS.MQ.RedisStream
{
    public class ConsumeContext
    {
        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        ///     依赖的Redis组件
        /// </summary>
        private readonly IRedisCacheManager _redisCacheManager;

        public ConsumeContext(IRedisCacheManager redisCacheManager, string queueName)
        {
            _redisCacheManager = redisCacheManager;
            _queueName         = queueName;
        }

        public string[] MessageIds { get; internal set; }

        /// <summary>
        ///     删除messageId
        /// </summary>
        public async Task Ack(StreamEntry message)
        {
            await _redisCacheManager.Db.StreamDeleteAsync(key: _queueName, messageIds: new[] { message.Id });
        }
    }
}