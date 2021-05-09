using System.Threading.Tasks;
using FS.Cache.Redis;
using FS.MQ.RedisStream.Configuration;
using Newtonsoft.Json;

namespace FS.MQ.RedisStream
{
    public class RedisStreamProduct : IRedisStreamProduct
    {
        private readonly ProductItemConfig  _productItemConfig;
        private readonly IRedisCacheManager _redisCacheManager;
        

        public RedisStreamProduct(IRedisCacheManager redisCacheManager, ProductItemConfig productItemConfig)
        {
            this._redisCacheManager = redisCacheManager;
            this._productItemConfig = productItemConfig;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message">消息主体</param>
        public bool Send(string message, string field = "data")
        {
            var redisValue = _redisCacheManager.Db.StreamAdd(_productItemConfig.QueueName, field, message);
            return redisValue.HasValue;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity">消息主体</param>
        public bool Send(object entity, string field = "data")
        {
            var json       = JsonConvert.SerializeObject(entity);
            var redisValue = _redisCacheManager.Db.StreamAdd(_productItemConfig.QueueName, field, json);
            return redisValue.HasValue;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message">消息主体</param>
        public async Task<bool> SendAsync(string message, string field = "data")
        {
            var redisValue = await _redisCacheManager.Db.StreamAddAsync(_productItemConfig.QueueName, field, message);
            return redisValue.HasValue;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity">消息主体</param>
        public async Task<bool> SendAsync(object entity, string field = "data")
        {
            var json       = JsonConvert.SerializeObject(entity);
            var redisValue = await _redisCacheManager.Db.StreamAddAsync(_productItemConfig.QueueName, field, json);
            return redisValue.HasValue;
        }
    }
}