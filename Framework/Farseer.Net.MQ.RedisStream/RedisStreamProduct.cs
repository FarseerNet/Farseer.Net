using System.Threading.Tasks;
using FS.Cache.Redis;
using FS.Core.Abstract.MQ;
using FS.Core.Abstract.MQ.RedisStream;
using FS.Core.AOP.LinkTrack;
using FS.Core.LinkTrack;
using FS.MQ.RedisStream.Configuration;
using Newtonsoft.Json;

namespace FS.MQ.RedisStream
{
    public class RedisStreamProduct : IRedisStreamProduct
    {
        private readonly ProductItemConfig  _productItemConfig;
        private readonly IRedisCacheManager _redisCacheManager;

        public string QueueName => _productItemConfig.QueueName;


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
        [TrackMqProduct(MqType.RedisStream)]
        public bool Send(string message, string field = "data")
        {
            var redisValue = _redisCacheManager.Db.StreamAdd(key: _productItemConfig.QueueName, streamField: field, streamValue: message, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
            return redisValue.HasValue;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity"> 消息主体 </param>
        [TrackMqProduct(MqType.RedisStream)]
        public bool Send(object entity, string field = "data")
        {
            var json       = JsonConvert.SerializeObject(value: entity);
            var redisValue = _redisCacheManager.Db.StreamAdd(key: _productItemConfig.QueueName, streamField: field, streamValue: json, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
            return redisValue.HasValue;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message"> 消息主体 </param>
        [TrackMqProduct(MqType.RedisStream)]
        public async Task<bool> SendAsync(string message, string field = "data")
        {
            var redisValue = await _redisCacheManager.Db.StreamAddAsync(key: _productItemConfig.QueueName, streamField: field, streamValue: message, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
            return redisValue.HasValue;
        }

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity"> 消息主体 </param>
        [TrackMqProduct(MqType.RedisStream)]
        public async Task<bool> SendAsync(object entity, string field = "data")
        {
            var json       = JsonConvert.SerializeObject(value: entity);
            var redisValue = await _redisCacheManager.Db.StreamAddAsync(key: _productItemConfig.QueueName, streamField: field, streamValue: json, maxLength: _productItemConfig.MaxLength, useApproximateMaxLength: true);
            return redisValue.HasValue;
        }
    }
}