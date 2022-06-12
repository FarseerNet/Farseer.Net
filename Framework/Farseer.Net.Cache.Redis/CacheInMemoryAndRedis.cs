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

    public IList Get(CacheKey cacheKey)
    {
        // 优先读本地缓存
        var list = _memoryCache.Get(cacheKey);
        if (list == null)
        {
            // 读redis
            list = _redisCache.Get(cacheKey);
            // 读到了，则写到本地缓存
            if (list != null)
            {
                _memoryCache.Set(cacheKey, list);
            }
        }
        return list;
    }

    public object GetItem(CacheKey cacheKey, string cacheId)
    {
        var item = _memoryCache.GetItem(cacheKey, cacheId);
        if (item == null)
        {
            item = _redisCache.GetItem(cacheKey, cacheId);
            if (item != null)
            {
                _memoryCache.SaveItem(cacheKey, item);
            }
        }
        return item;
    }

    public void Set(CacheKey cacheKey, IList lst)
    {
        _redisCache.Set(cacheKey, lst);
        _memoryCache.Set(cacheKey, lst);
    }

    public int Count(CacheKey cacheKey)
    {
        var count = _memoryCache.Count(cacheKey);
        if (count > 0) return count;

        count = _redisCache.Count(cacheKey);
        if (count > 0) return count;

        return 0;
    }

    public bool ExistsItem(CacheKey cacheKey, string cacheId)
    {
        if (_memoryCache.ExistsItem(cacheKey, cacheId)) return true;
        if (_redisCache.ExistsItem(cacheKey, cacheId)) return true;
        return false;
    }

    public bool ExistsKey(CacheKey cacheKey)
    {
        if (_memoryCache.ExistsKey(cacheKey)) return true;
        if (_redisCache.ExistsKey(cacheKey)) return true;
        return false;
    }

    public void SaveItem(CacheKey cacheKey, object newVal)
    {
        _redisCache.SaveItem(cacheKey, newVal);
        _memoryCache.SaveItem(cacheKey, newVal);
    }

    public void Remove(CacheKey cacheKey, string cacheId)
    {
        _redisCache.Remove(cacheKey, cacheId);
        _memoryCache.Remove(cacheKey, cacheId);
    }

    public void Clear(CacheKey cacheKey)
    {
        _redisCache.Clear(cacheKey);
        _memoryCache.Clear(cacheKey);
    }
}