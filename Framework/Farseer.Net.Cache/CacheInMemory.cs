using System.Collections;
using FS.Core.Abstract.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace FS.Cache;

public class CacheInMemory : ICache
{
    internal static readonly MemoryCache MyCache = new(optionsAccessor: new MemoryCacheOptions());

    public IList Get(CacheKey cacheKey) => MyCache.Get<IList>(cacheKey.Key);

    public object GetItem(CacheKey cacheKey, string cacheId)
    {
        var list = Get(cacheKey);
        if (list == null) return false;

        foreach (var item in list)
        {
            // 从当前缓存item中，获取唯一标识
            var itemDataKey = PropertyGetCacheManger.Cache(cacheKey.DataKey, item).ToString();

            // 找到了
            if (itemDataKey == cacheId) return item;
        }
        return null;
    }

    public void Set(CacheKey cacheKey, IList val)
    {
        using (var cacheEntry = MyCache.CreateEntry(cacheKey.Key))
        {
            // 设置过期时间
            if (cacheKey.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheKey.MemoryExpiry.GetValueOrDefault();
            cacheEntry.Value = val;
        }
    }

    public int Count(CacheKey cacheKey) => Get(cacheKey)?.Count ?? 0;

    public bool ExistsItem(CacheKey cacheKey, string cacheId)
    {
        var list = Get(cacheKey);
        if (list == null) return false;

        foreach (var item in list)
        {
            // 从当前缓存item中，获取唯一标识
            var itemDataKey = PropertyGetCacheManger.Cache(cacheKey.DataKey, item).ToString();

            // 找到了
            if (itemDataKey == cacheId)
            {
                return true;
            }
        }
        return false;
    }

    public bool ExistsKey(CacheKey cacheKey)
    {
        var list = Get(cacheKey);
        return list != null;
    }

    public void SaveItem(CacheKey cacheKey, object newVal)
    {
        var list = Get(cacheKey);
        if (list == null) return;

        // key.DataKey=null，说明实际缓存的是单个对象。所以此处直接替换新的对象即可，而不用查找。
        if (cacheKey.DataKey == null) list.Clear();
        else
        {
            // 从新对象中，获取唯一标识
            var newValDataKey = PropertyGetCacheManger.Cache(cacheKey.DataKey, newVal).ToString();

            for (var index = 0; index < list.Count; index++)
            {
                // 从当前缓存item中，获取唯一标识
                var itemDataKey = PropertyGetCacheManger.Cache(cacheKey.DataKey, list[index]).ToString();

                // 找到了
                if (itemDataKey == newValDataKey)
                {
                    list[index] = newVal;
                    return;
                }
            }
        }
        list.Add(newVal);
    }

    public void Remove(CacheKey cacheKey, string cacheId)
    {
        var list = Get(cacheKey);
        if (list == null) return;

        // key.DataKey=null，说明实际缓存的是单个对象。所以此处直接清空即可。
        if (cacheKey.DataKey == null) Clear(cacheKey);
        else
        {
            for (var index = 0; index < list.Count; index++)
            {
                // 从当前缓存item中，获取唯一标识
                var itemDataKey = PropertyGetCacheManger.Cache(cacheKey.DataKey, list[index]).ToString();

                // 找到了
                if (itemDataKey == cacheId)
                {
                    list.RemoveAt(index);
                    return;
                }
            }
        }
    }

    public void Clear(CacheKey cacheKey) => MyCache.Remove(cacheKey.Key);
}