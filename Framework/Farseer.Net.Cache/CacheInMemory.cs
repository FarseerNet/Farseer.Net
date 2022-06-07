using System.Collections;
using FS.Core.Abstract.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace FS.Cache;

public class CacheInMemory : ICache
{
    internal static readonly MemoryCache MyCache = new(optionsAccessor: new MemoryCacheOptions());

    /// <summary>
    /// 从本地内存中获取
    /// </summary>
    public IList Get(CacheKey2 key) => MyCache.Get<IList>(key.Key);

    /// <summary>
    /// 保存数据到缓存
    /// </summary>
    /// <param name="key">缓存key</param>
    /// <param name="val">要缓存的数据</param>
    public void Set(CacheKey2 key, IList val)
    {
        using (var cacheEntry = MyCache.CreateEntry(key.Key))
        {
            // 设置过期时间
            if (key.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = key.MemoryExpiry.GetValueOrDefault();
            cacheEntry.Value = val;
        }
    }

    /// <summary>
    /// 保存数据到缓存
    /// </summary>
    /// <param name="key">缓存key</param>
    /// <param name="newVal">要缓存的数据</param>
    public void Save(CacheKey2 key, object newVal)
    {
        var list = Get(key);
        if (list == null) return;

        // key.DataKey=null，说明实际缓存的是单个对象。所以此处直接替换新的对象即可，而不用查找。
        if (key.DataKey == null) list.Clear();
        else
        {
            // 从新对象中，获取唯一标识
            var newValDataKey = PropertyGetCacheManger.Cache(key.DataKey, newVal).ToString();

            for (var index = 0; index < list.Count; index++)
            {
                // 从当前缓存item中，获取唯一标识
                var itemDataKey = PropertyGetCacheManger.Cache(key.DataKey, list[index]).ToString();

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

    public void Remove(CacheKey2 key, string cacheId)
    {
        var list = Get(key);
        if (list == null) return;

        // key.DataKey=null，说明实际缓存的是单个对象。所以此处直接清空即可。
        if (key.DataKey == null) Clear(key);
        else
        {
            for (var index = 0; index < list.Count; index++)
            {
                // 从当前缓存item中，获取唯一标识
                var itemDataKey = PropertyGetCacheManger.Cache(key.DataKey, list[index]).ToString();

                // 找到了
                if (itemDataKey == cacheId)
                {
                    list.RemoveAt(index);
                    return;
                }
            }
        }
    }

    public void Clear(CacheKey2 key) => MyCache.Remove(key.Key);
}