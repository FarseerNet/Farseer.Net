using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core;
using FS.DI;
using Newtonsoft.Json;

namespace FS.Cache.Redis;

public class GetCacheInRedis : IGetCache
{
    private readonly IRedisCacheManager _redisCacheManager;

    public GetCacheInRedis(string redisItemConfigName)
    {
        _redisCacheManager = IocManager.GetService<IRedisCacheManager>(name: redisItemConfigName);
    }

    /// <summary>
    ///     从缓存中读取LIST
    /// </summary>
    public List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
    {
        var hashGetAll = _redisCacheManager.Db.HashGetAll(key: cacheKey.Key);
        return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject<TEntity>(value: o.Value)).ToList();
    }

    /// <summary>
    ///     从缓存中读取LIST
    /// </summary>
    public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
    {
        var hashGetAll = await _redisCacheManager.Db.HashGetAllAsync(key: cacheKey.Key);
        return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject<TEntity>(value: o.Value)).ToList();
    }

    /// <summary>
    ///     从缓存中读取实体
    /// </summary>
    public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        var redisValue = _redisCacheManager.Db.HashGet(key: cacheKey.Key, hashField: fieldKey.ToString());
        return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(value: redisValue.ToString());
    }

    /// <summary>
    ///     从缓存中读取实体
    /// </summary>
    public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        var redisValue = await _redisCacheManager.Db.HashGetAsync(key: cacheKey.Key, hashField: fieldKey.ToString());
        return !redisValue.HasValue ? default : JsonConvert.DeserializeObject<TEntity>(value: redisValue.ToString());
    }

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public bool Exists(CacheKey cacheKey) => _redisCacheManager.Db.KeyExists(key: cacheKey.Key);

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public Task<bool> ExistsAsync(CacheKey cacheKey) => _redisCacheManager.Db.KeyExistsAsync(key: cacheKey.Key);

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
    public bool ExistsItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey) => _redisCacheManager.Db.HashExists(key: cacheKey.Key, hashField: fieldKey.ToString());

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
    public Task<bool> ExistsItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey) => _redisCacheManager.Db.HashExistsAsync(key: cacheKey.Key, hashField: fieldKey.ToString());

    /// <summary>
    ///     获取集合的数量
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public long GetCount(CacheKey cacheKey) => _redisCacheManager.Db.HashLength(key: cacheKey.Key);

    /// <summary>
    ///     获取集合的数量
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public Task<long> GetCountAsync(CacheKey cacheKey) => _redisCacheManager.Db.HashLengthAsync(key: cacheKey.Key);

    /// <summary>
    ///     将实体保存到缓存中
    /// </summary>
    public void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
    {
        var transaction = _redisCacheManager.Db.CreateTransaction();
        transaction.HashSetAsync(key: cacheKey.Key, hashField: cacheKey.GetField(arg: entity).ToString(), value: JsonConvert.SerializeObject(value: entity));

        // 设置过期时间
        if (cacheKey.RedisExpiry != null) transaction.KeyExpireAsync(key: cacheKey.Key, expiry: cacheKey.RedisExpiry.GetValueOrDefault());

        transaction.Execute();
    }

    /// <summary>
    ///     将实体保存到缓存中
    /// </summary>
    public Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
    {
        var transaction = _redisCacheManager.Db.CreateTransaction();
        transaction.HashSetAsync(key: cacheKey.Key, hashField: cacheKey.GetField(arg: entity).ToString(), value: JsonConvert.SerializeObject(value: entity));

        // 设置过期时间
        if (cacheKey.RedisExpiry != null) transaction.KeyExpireAsync(key: cacheKey.Key, expiry: cacheKey.RedisExpiry.GetValueOrDefault());

        return transaction.ExecuteAsync();
    }

    /// <summary>
    ///     将LIST保存到缓存中
    /// </summary>
    public void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst)
    {
        _redisCacheManager.HashSetTransaction(key: cacheKey.Key, lst: lst, funcDataKey: cacheKey.GetField, funcData: null, expiry: cacheKey.RedisExpiry);
    }

    /// <summary>
    ///     将LIST保存到缓存中
    /// </summary>
    public Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst) => _redisCacheManager.HashSetTransactionAsync(key: cacheKey.Key, lst: lst, funcDataKey: cacheKey.GetField, funcData: null, expiry: cacheKey.RedisExpiry);


    /// <summary>
    ///     删除整个缓存
    /// </summary>
    public void Remove(CacheKey cacheKey) => _redisCacheManager.Db.KeyDelete(key: cacheKey.Key);

    /// <summary>
    ///     删除整个缓存
    /// </summary>
    public Task RemoveAsync(CacheKey cacheKey) => _redisCacheManager.Db.KeyDeleteAsync(key: cacheKey.Key);

    /// <summary>
    ///     删除缓存item
    /// </summary>
    public void Remove<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey) => _redisCacheManager.Db.HashDelete(key: cacheKey.Key, hashField: fieldKey.ToString());

    /// <summary>
    ///     删除缓存item
    /// </summary>
    public Task RemoveAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey) => _redisCacheManager.Db.HashDeleteAsync(key: cacheKey.Key, hashField: fieldKey.ToString());


    /// <summary>
    ///     从缓存集合中读取实体
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public TEntity Get<TEntity>(CacheKey<TEntity> cacheKey)
    {
        var redisValue = _redisCacheManager.Db.StringGet(key: cacheKey.Key);
        if (redisValue.HasValue) return Jsons.ToObject<TEntity>(obj: redisValue.ToString());
        return default;
    }

    /// <summary>
    ///     从缓存集合中读取实体
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public async Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey)
    {
        var redisValue = await _redisCacheManager.Db.StringGetAsync(key: cacheKey.Key);
        if (redisValue.HasValue) return Jsons.ToObject<TEntity>(obj: redisValue.ToString());
        return default;
    }

    /// <summary>
    ///     保存对象
    /// </summary>
    /// <param name="entity"> 保存对象 </param>
    /// <param name="cacheKey"> 缓存策略 </param>
    public void Save<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
    {
        if (cacheKey.RedisExpiry != null)
            _redisCacheManager.Db.StringSet(key: cacheKey.Key, value: JsonConvert.SerializeObject(value: entity), expiry: cacheKey.RedisExpiry.GetValueOrDefault());
        else
            _redisCacheManager.Db.StringSet(key: cacheKey.Key, value: JsonConvert.SerializeObject(value: entity));
    }

    /// <summary>
    ///     保存对象
    /// </summary>
    /// <param name="entity"> 保存对象 </param>
    /// <param name="cacheKey"> 缓存策略 </param>
    public Task SaveAsync<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
    {
        if (cacheKey.RedisExpiry != null) return _redisCacheManager.Db.StringSetAsync(key: cacheKey.Key, value: JsonConvert.SerializeObject(value: entity), expiry: cacheKey.RedisExpiry.GetValueOrDefault());

        return _redisCacheManager.Db.StringSetAsync(key: cacheKey.Key, value: JsonConvert.SerializeObject(value: entity));
    }
}