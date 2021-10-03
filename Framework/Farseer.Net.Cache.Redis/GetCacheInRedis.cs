using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core;
using FS.DI;
using Newtonsoft.Json;

namespace FS.Cache.Redis
{
    public class GetCacheInRedis : IGetCache
    {
        private readonly IRedisCacheManager _redisCacheManager;

        public GetCacheInRedis(string redisItemConfigName)
        {
            _redisCacheManager = IocManager.Instance.Resolve<IRedisCacheManager>(name: redisItemConfigName);
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public List<TEntity> GetList<TEntity, TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId)
        {
            var hashGetAll = _redisCacheManager.Db.HashGetAll(cacheKey.Key);
            return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject<TEntity>(value: o.Value)).ToList();
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId)
        {
            var hashGetAll = await _redisCacheManager.Db.HashGetAllAsync(cacheKey.Key);
            return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject<TEntity>(value: o.Value)).ToList();
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId)
        {
            var redisValue = _redisCacheManager.Db.HashGet(cacheKey.Key, hashField: fieldKey.ToString());
            return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(value: redisValue.ToString());
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId)
        {
            var redisValue = await _redisCacheManager.Db.HashGetAsync(cacheKey.Key, hashField: fieldKey.ToString());
            return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(value: redisValue.ToString());
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public bool ExistsItem<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            return _redisCacheManager.Db.HashExists(cacheKey.Key, fieldKey.ToString());
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public bool Exists(CacheKey cacheKey)
        {
            return _redisCacheManager.Db.KeyExists(cacheKey.Key);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<bool> ExistsAsync(CacheKey cacheKey)
        {
            return _redisCacheManager.Db.KeyExistsAsync(cacheKey.Key);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public Task<bool> ExistsItemAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            return _redisCacheManager.Db.HashExistsAsync(cacheKey.Key, fieldKey.ToString());
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public long GetCount(CacheKey cacheKey)
        {
            return _redisCacheManager.Db.HashLength(cacheKey.Key);
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<long> GetCountAsync(CacheKey cacheKey)
        {
            return _redisCacheManager.Db.HashLengthAsync(cacheKey.Key);
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId)
        {
            var transaction = _redisCacheManager.Db.CreateTransaction();
            transaction.HashSetAsync(cacheKey.Key, hashField: getEntityId(arg: entity).ToString(), value: JsonConvert.SerializeObject(value: entity));

            // 设置过期时间
            if (cacheKey.RedisExpiry != null) transaction.KeyExpireAsync(cacheKey.Key, expiry: cacheKey.RedisExpiry.GetValueOrDefault());

            transaction.Execute();
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public Task SaveItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId)
        {
            var transaction = _redisCacheManager.Db.CreateTransaction();
            transaction.HashSetAsync(cacheKey.Key, hashField: getEntityId(arg: entity).ToString(), value: JsonConvert.SerializeObject(value: entity));

            // 设置过期时间
            if (cacheKey.RedisExpiry != null) transaction.KeyExpireAsync(cacheKey.Key, expiry: cacheKey.RedisExpiry.GetValueOrDefault());

            return transaction.ExecuteAsync();
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            _redisCacheManager.HashSetTransaction(cacheKey.Key, lst: lst, getEntityId, funcData: null, expiry: cacheKey.RedisExpiry);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public Task SaveListAsync<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId) => _redisCacheManager.HashSetTransactionAsync(cacheKey.Key, lst: lst, funcDataKey: getEntityId, funcData: null, expiry: cacheKey.RedisExpiry);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove<TEntityId>(CacheKey cacheKey, TEntityId fieldKey) => _redisCacheManager.Db.HashDelete(cacheKey.Key, hashField: fieldKey.ToString());

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public Task RemoveAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey) => _redisCacheManager.Db.HashDeleteAsync(cacheKey.Key, hashField: fieldKey.ToString());

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public void Remove(CacheKey cacheKey) => _redisCacheManager.Db.KeyDelete(cacheKey.Key);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public Task RemoveAsync(CacheKey cacheKey) => _redisCacheManager.Db.KeyDeleteAsync(cacheKey.Key);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey cacheKey)
        {
            var redisValue = _redisCacheManager.Db.StringGet(cacheKey.Key);
            if (redisValue.HasValue) return Jsons.ToObject<TEntity>(obj: redisValue.ToString());
            return default;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey)
        {
            var redisValue = await _redisCacheManager.Db.StringGetAsync(cacheKey.Key);
            if (redisValue.HasValue) return Jsons.ToObject<TEntity>(obj: redisValue.ToString());
            return default;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Save<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            if (cacheKey.RedisExpiry != null)
                _redisCacheManager.Db.StringSet(cacheKey.Key, value: JsonConvert.SerializeObject(value: entity), expiry: cacheKey.RedisExpiry.GetValueOrDefault());
            else
                _redisCacheManager.Db.StringSet(cacheKey.Key, value: JsonConvert.SerializeObject(value: entity));
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            if (cacheKey.RedisExpiry != null) return _redisCacheManager.Db.StringSetAsync(cacheKey.Key, value: JsonConvert.SerializeObject(value: entity), expiry: cacheKey.RedisExpiry.GetValueOrDefault());

            return _redisCacheManager.Db.StringSetAsync(cacheKey.Key, value: JsonConvert.SerializeObject(value: entity));
        }
    }
}