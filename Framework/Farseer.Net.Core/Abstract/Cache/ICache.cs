using System.Collections;

namespace FS.Core.Abstract.Cache;

public interface ICache
{
    /// <summary>
    /// 从本地内存中获取
    /// </summary>
    IList Get(CacheKey2 key);
    
    /// <summary>
    /// 保存数据到缓存
    /// </summary>
    /// <param name="key">缓存key</param>
    /// <param name="val">要缓存的数据</param>
    void Set(CacheKey2 key, IList val);
    /// <summary>
    /// 保存数据到缓存
    /// </summary>
    /// <param name="key">缓存key</param>
    /// <param name="newVal">要缓存的数据</param>
    void Save(CacheKey2 key, object newVal);
    
    /// <summary>
    /// 移除缓存
    /// </summary>
    void Remove(CacheKey2 key, string cacheId);
    
    /// <summary>
    /// 清空缓存
    /// </summary>
    void Clear(CacheKey2 key);
}