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
            _redisCacheManager = redisCacheManager;
            _productItemConfig = productItemConfig;
            if (_productItemConfig.MaxLength == 0) _productItemConfig.MaxLength = 9999999;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message"> 消息主体 </param>
        public bool Send(string message, string field = "data")
        {
            using (FsLinkTrack.TrackMqProduct(method: $"RedisStream.Send.{_productItemConfig.QueueName}"))
            {
                var redisValue = _redisCacheManager.Db.StreamAdd(key: _productItemConfig.QueueName, streamField: field, streamValue: message, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity"> 消息主体 </param>
        public bool Send(object entity, string field = "data")
        {
            using (FsLinkTrack.TrackMqProduct(method: $"RedisStream.Send.{_productItemConfig.QueueName}"))
            {
                var json       = JsonConvert.SerializeObject(value: entity);
                var redisValue = _redisCacheManager.Db.StreamAdd(key: _productItemConfig.QueueName, streamField: field, streamValue: json, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message"> 消息主体 </param>
        public async Task<bool> SendAsync(string message, string field = "data")
        {
            using (FsLinkTrack.TrackMqProduct(method: $"RedisStream.Send.{_productItemConfig.QueueName}"))
            {
                var redisValue = await _redisCacheManager.Db.StreamAddAsync(key: _productItemConfig.QueueName, streamField: field, streamValue: message, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity"> 消息主体 </param>
        public async Task<bool> SendAsync(object entity, string field = "data")
        {
            using (FsLinkTrack.TrackMqProduct(method: $"RedisStream.Send.{_productItemConfig.QueueName}"))
            {
                var json       = JsonConvert.SerializeObject(value: entity);
                var redisValue = await _redisCacheManager.Db.StreamAddAsync(key: _productItemConfig.QueueName, streamField: field, streamValue: json, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
                return redisValue.HasValue;
            }
        }
    }
}