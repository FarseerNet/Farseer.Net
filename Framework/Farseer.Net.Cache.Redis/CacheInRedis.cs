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

    public IList Get(CacheKey2 key)
    {
        var hashGetAll = _redisCacheManager.Db.HashGetAll(key.Key);
        if (hashGetAll.Length == 0) return null;
        return hashGetAll.Select(selector: o => JsonConvert.DeserializeObject(o.Value, key.ItemType)).ToPooledList();
    }

    public void Set(CacheKey2 key, IList lst)
    {
        var       transaction = _redisCacheManager.Db.CreateTransaction();
        using var tasks       = new PooledList<Task>();

        foreach (var val in lst)
        {
            var dataKey = key.DataKey != null ? PropertyGetCacheManger.Cache(key.DataKey, val).ToString() : val.GetType().Name;
            var data    = JsonConvert.SerializeObject(val);
            tasks.Add(item: transaction.HashSetAsync(key: key.Key, hashField: dataKey, value: data));
        }

        // 设置过期时间
        if (key.RedisExpiry != null) transaction.KeyExpireAsync(key: key.Key, expiry: key.RedisExpiry.GetValueOrDefault());
        transaction.Execute();
        Task.WaitAll(tasks: tasks.ToArray());
    }

    public void Save(CacheKey2 key, object newVal)
    {
        var dataKey = key.DataKey != null ? PropertyGetCacheManger.Cache(key.DataKey, newVal).ToString() : newVal.GetType().Name;
        var data    = JsonConvert.SerializeObject(newVal);
        _redisCacheManager.Db.HashSet(key.Key, dataKey, data);
    }

    public void Remove(CacheKey2 key, string cacheId)
    {
        _redisCacheManager.Db.HashDelete(key.Key, cacheId);
    }

    public void Clear(CacheKey2 key)
    {
        _redisCacheManager.Db.KeyDelete(key.Key);
    }
}