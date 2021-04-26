using System;
using System.Collections.Generic;
using System.Linq;
using FS.DI;
using Newtonsoft.Json;

namespace FS.Cache.Redis
{
    public class GetCacheByRedis : IGetCache
    {
        private readonly IRedisCacheManager _redisCacheManager;

        public GetCacheByRedis(IRedisCacheManager redisCacheManager)
        {
            this._redisCacheManager = redisCacheManager;
        }

        /// <summary>
        /// 从缓存中读取LIST
        /// </summary>
        /// <param name="key">缓存Key</param>
        public List<TEntity> ToList<TEntity>(string key)
        {
            var hashGetAll = _redisCacheManager.Db.HashGetAll(key);
            return hashGetAll.Select(o => JsonConvert.DeserializeObject<TEntity>(o.Value)).ToList();
        }

        /// <summary>
        /// 从缓存中读取实体
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        public TEntity ToEntity<TEntity>(string key, string fieldKey)
        {
            var redisValue = _redisCacheManager.Db.HashGet(key, fieldKey);
            return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(redisValue.ToString());
        }

        /// <summary>
        /// 将实体保存到缓存中
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="entity">数据源</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public void Save<TEntity>(string key, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            _redisCacheManager.Db.HashSet(key, getEntityId(entity).ToString(), JsonConvert.SerializeObject(entity));
        }

        /// <summary>
        /// 将LIST保存到缓存中
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="lst">数据源</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public void Save<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            _redisCacheManager.HashSetTransaction(key, lst, getEntityId);
        }
    }
}