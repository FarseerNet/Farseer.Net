using System.Threading.Tasks;
using FS.Cache.Redis;
using FS.Core.LinkTrack;
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
            if (this._productItemConfig.MaxLength == 0) this._productItemConfig.MaxLength = 9999999;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message">消息主体</param>
        public bool Send(string message, string field = "data")
        {
            using (FsLinkTrack.TrackMq("RedisStream.Send"))
            {
                var redisValue = _redisCacheManager.Db.StreamAdd(_productItemConfig.QueueName, field, message, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity">消息主体</param>
        public bool Send(object entity, string field = "data")
        {
            using (FsLinkTrack.TrackMq("RedisStream.Send"))
            {
                var json       = JsonConvert.SerializeObject(entity);
                var redisValue = _redisCacheManager.Db.StreamAdd(_productItemConfig.QueueName, field, json, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message">消息主体</param>
        public async Task<bool> SendAsync(string message, string field = "data")
        {
            using (FsLinkTrack.TrackMq("RedisStream.Send"))
            {
                var redisValue = await _redisCacheManager.Db.StreamAddAsync(_productItemConfig.QueueName, field, message, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity">消息主体</param>
        public async Task<bool> SendAsync(object entity, string field = "data")
        {
            using (FsLinkTrack.TrackMq("RedisStream.Send"))
            {
                var json       = JsonConvert.SerializeObject(entity);
                var redisValue = await _redisCacheManager.Db.StreamAddAsync(_productItemConfig.QueueName, field, json, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }
    }
}