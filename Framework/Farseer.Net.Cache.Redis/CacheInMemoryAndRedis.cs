using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Abstract.Cache;
using FS.DI;
using Newtonsoft.Json;

namespace FS.Cache.Redis;

public class CacheInMemoryAndRedis : ICache
{
    private readonly ICache _memoryCache;
    private readonly ICache _redisCache;

    public CacheInMemoryAndRedis(string redisItemConfigName)
    {
        _redisCache  = IocManager.GetService<ICache>(name: $"CacheInRedis_{redisItemConfigName}");
        _memoryCache = IocManager.GetService<ICache>(name: "CacheInMemory");
    }

    public IList Get(CacheKey2 key)
    {
        // 优先读本地缓存
        var list = _memoryCache.Get(key);
        if (list == null)
        {
            // 读redis
            list = _redisCache.Get(key);
            // 读到了，则写到本地缓存
            if (list != null)
            {
                _memoryCache.Set(key, list);
            }
        }
        return list;
    }

    public void Set(CacheKey2 key, IList lst)
    {
        _redisCache.Set(key, lst);
        _memoryCache.Set(key, lst);
    }

    public void Save(CacheKey2 key, object newVal)
    {
        _redisCache.Save(key, newVal);
        _memoryCache.Save(key, newVal);
    }

    public void Remove(CacheKey2 key, string cacheId)
    {
        _redisCache.Remove(key, cacheId);
        _memoryCache.Remove(key, cacheId);
    }

    public void Clear(CacheKey2 key)
    {
        _redisCache.Clear(key);
        _memoryCache.Clear(key);
    }
}