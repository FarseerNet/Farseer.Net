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
        public List<TEntity> GetList<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var hashGetAll = _redisCacheManager.Db.HashGetAll(key: cacheKey);
            return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject<TEntity>(value: o.Value)).ToList();
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public async Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var hashGetAll = await _redisCacheManager.Db.HashGetAllAsync(key: cacheKey);
            return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject<TEntity>(value: o.Value)).ToList();
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var redisValue = _redisCacheManager.Db.HashGet(key: cacheKey, hashField: fieldKey);
            return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(value: redisValue.ToString());
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public async Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var redisValue = await _redisCacheManager.Db.HashGetAsync(key: cacheKey, hashField: fieldKey);
            return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(value: redisValue.ToString());
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var transaction = _redisCacheManager.Db.CreateTransaction();
            transaction.HashSetAsync(key: cacheKey, hashField: getEntityId(arg: entity).ToString(), value: JsonConvert.SerializeObject(value: entity));

            // 设置过期时间
            if (cacheOption.RedisExpiry != null) transaction.KeyExpireAsync(key: cacheKey, expiry: cacheOption.RedisExpiry.GetValueOrDefault());

            transaction.Execute();
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public Task SaveItemAsync<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var transaction = _redisCacheManager.Db.CreateTransaction();
            transaction.HashSetAsync(key: cacheKey, hashField: getEntityId(arg: entity).ToString(), value: JsonConvert.SerializeObject(value: entity));

            // 设置过期时间
            if (cacheOption.RedisExpiry != null) transaction.KeyExpireAsync(key: cacheKey, expiry: cacheOption.RedisExpiry.GetValueOrDefault());

            return transaction.ExecuteAsync();
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            _redisCacheManager.HashSetTransaction(key: cacheKey, lst: lst, funcDataKey: getEntityId, funcData: null, expiry: cacheOption.RedisExpiry);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public Task SaveListAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption) => _redisCacheManager.HashSetTransactionAsync(key: cacheKey, lst: lst, funcDataKey: getEntityId, funcData: null, expiry: cacheOption.RedisExpiry);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove(string cacheKey, string fieldKey) => _redisCacheManager.Db.HashDelete(key: cacheKey, hashField: fieldKey);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public Task RemoveAsync(string cacheKey, string fieldKey) => _redisCacheManager.Db.HashDeleteAsync(key: cacheKey, hashField: fieldKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public void Remove(string cacheKey) => _redisCacheManager.Db.KeyDelete(key: cacheKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public Task RemoveAsync(string cacheKey) => _redisCacheManager.Db.KeyDeleteAsync(key: cacheKey);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public TEntity Get<TEntity>(string cacheKey, CacheOption cacheOption)
        {
            var redisValue = _redisCacheManager.Db.StringGet(key: cacheKey);
            if (redisValue.HasValue) return Jsons.ToObject<TEntity>(obj: redisValue.ToString());
            return default;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(string cacheKey, CacheOption cacheOption)
        {
            var redisValue = await _redisCacheManager.Db.StringGetAsync(key: cacheKey);
            if (redisValue.HasValue) return Jsons.ToObject<TEntity>(obj: redisValue.ToString());
            return default;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public void Save<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            if (cacheOption.RedisExpiry != null)
                _redisCacheManager.Db.StringSet(key: cacheKey, value: JsonConvert.SerializeObject(value: entity), expiry: cacheOption.RedisExpiry.GetValueOrDefault());
            else
                _redisCacheManager.Db.StringSet(key: cacheKey, value: JsonConvert.SerializeObject(value: entity));
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            if (cacheOption.RedisExpiry != null) return _redisCacheManager.Db.StringSetAsync(key: cacheKey, value: JsonConvert.SerializeObject(value: entity), expiry: cacheOption.RedisExpiry.GetValueOrDefault());

            return _redisCacheManager.Db.StringSetAsync(key: cacheKey, value: JsonConvert.SerializeObject(value: entity));
        }
    }
}