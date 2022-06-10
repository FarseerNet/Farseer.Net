using System.Collections;

namespace FS.Core.Abstract.Cache;

public interface ICache
{
    /// <summary>
    /// 从本地内存中获取
    /// </summary>
    IList Get(CacheKey key);
    
    /// <summary>
    /// 保存数据到缓存
    /// </summary>
    /// <param name="key">缓存key</param>
    /// <param name="val">要缓存的数据</param>
    void Set(CacheKey key, IList val);
    /// <summary>
    /// 保存数据到缓存
    /// </summary>
    /// <param name="key">缓存key</param>
    /// <param name="newVal">要缓存的数据</param>
    void Save(CacheKey key, object newVal);
    
    /// <summary>
    /// 移除缓存
    /// </summary>
    void Remove(CacheKey key, string cacheId);
    
    /// <summary>
    /// 清空缓存
    /// </summary>
    void Clear(CacheKey key);

    /// <summary>
    /// 获取集合数量
    /// </summary>
    int Count(CacheKey key);

    /// <summary>
    /// 指定数据是否存在
    /// </summary>
    bool Exists(CacheKey key, string cacheId);
}