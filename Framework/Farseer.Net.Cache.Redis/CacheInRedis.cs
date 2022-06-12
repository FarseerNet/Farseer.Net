using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Abstract.Cache;
using FS.DI;
using Newtonsoft.Json;

namespace FS.Cache.Redis;

public class CacheInRedis : ICache
{
    private readonly IRedisCacheManager _redisCacheManager;

    public CacheInRedis(string redisItemConfigName)
    {
        _redisCacheManager = IocManager.GetService<IRedisCacheManager>(name: redisItemConfigName);
    }

    public IList Get(CacheKey cacheKey)
    {
        var hashGetAll = _redisCacheManager.Db.HashGetAll(cacheKey.Key);
        if (hashGetAll.Length == 0) return null;
        return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject(o.Value, cacheKey.ItemType)).ToPooledList();
    }
    
    public object GetItem(CacheKey cacheKey, string cacheId)
    {
        var redisValue = _redisCacheManager.Db.HashGet(cacheKey.Key,cacheId);
        return redisValue.HasValue ? JsonConvert.DeserializeObject(redisValue, cacheKey.ItemType) : null;
    }
    
    public void Set(CacheKey cacheKey, IList lst)
    {
        var       transaction = _redisCacheManager.Db.CreateTransaction();
        using var tasks       = new PooledList<Task>();

        foreach (var val in lst)
        {
            var dataKey = cacheKey.DataKey != null ? PropertyGetCacheManger.Cache(cacheKey.DataKey, val).ToString() : val.GetType().Name;
            var data    = JsonConvert.SerializeObject(val);
            tasks.Add(item: transaction.HashSetAsync(key: cacheKey.Key, hashField: dataKey, value: data));
        }

        // 设置过期时间
        if (cacheKey.RedisExpiry != null) transaction.KeyExpireAsync(key: cacheKey.Key, expiry: cacheKey.RedisExpiry.GetValueOrDefault());
        transaction.Execute();
        Task.WaitAll(tasks: tasks.ToArray());
    }

    public int Count(CacheKey cacheKey) => (int)_redisCacheManager.Db.HashLength(cacheKey.Key);

    public bool ExistsItem(CacheKey cacheKey, string cacheId) => _redisCacheManager.Db.HashExists(cacheKey.Key, cacheId);

    public bool ExistsKey(CacheKey cacheKey) => _redisCacheManager.Db.KeyExists(cacheKey.Key);


    public void SaveItem(CacheKey cacheKey, object newVal)
    {
        var dataKey = cacheKey.DataKey != null ? PropertyGetCacheManger.Cache(cacheKey.DataKey, newVal).ToString() : newVal.GetType().Name;
        var data    = JsonConvert.SerializeObject(newVal);
        _redisCacheManager.Db.HashSet(cacheKey.Key, dataKey, data);
    }

    public void Remove(CacheKey cacheKey, string cacheId)
    {
        _redisCacheManager.Db.HashDelete(cacheKey.Key, cacheId);
    }

    public void Clear(CacheKey cacheKey)
    {
        _redisCacheManager.Db.KeyDelete(cacheKey.Key);
    }
}