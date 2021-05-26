using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FS.DI;
using Microsoft.Extensions.Logging;
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
        /// 从缓存中读取LIST
        /// </summary>
        /// <param name="key">缓存Key</param>
        public async Task<List<TEntity>> ToListAsync<TEntity>(string key)
        {
            var hashGetAll = await _redisCacheManager.Db.HashGetAllAsync(key);
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
        /// 从缓存中读取实体
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        public async Task<TEntity> ToEntityAsync<TEntity>(string key, string fieldKey)
        {
            var redisValue = await _redisCacheManager.Db.HashGetAsync(key, fieldKey);
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
        /// 将实体保存到缓存中
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="entity">数据源</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public Task SaveAsync<TEntity>(string key, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            return _redisCacheManager.Db.HashSetAsync(key, getEntityId(entity).ToString(), JsonConvert.SerializeObject(entity));
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

        /// <summary>
        /// 将LIST保存到缓存中
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="lst">数据源</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public Task SaveAsync<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            return _redisCacheManager.HashSetTransactionAsync(key, lst, getEntityId);
        }

        /// <summary>
        /// 删除缓存item
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        /// <param name="fieldKey">缓存Field</param>
        public void Remove(string cacheKey, string fieldKey)
        {
            _redisCacheManager.Db.HashDelete(cacheKey, fieldKey);
        }

        /// <summary>
        /// 删除缓存item
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        /// <param name="fieldKey">缓存Field</param>
        public Task RemoveAsync(string cacheKey, string fieldKey)
        {
            return _redisCacheManager.Db.HashDeleteAsync(cacheKey, fieldKey);
        }

        /// <summary>
        /// 删除整个缓存
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        public void Remove(string cacheKey)
        {
            _redisCacheManager.Db.KeyDelete(cacheKey);
        }

        /// <summary>
        /// 删除整个缓存
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        public Task RemoveAsync(string cacheKey)
        {
            return _redisCacheManager.Db.KeyDeleteAsync(cacheKey);
        }
    }
}