using System.Collections;

namespace FS.Core.Abstract.Cache;

public interface ICache
{
    /// <summary>
    /// 从本地内存中获取
    /// </summary>
    IList Get(CacheKey cacheKey);
    
    /// <summary>
    /// 获取集合中的item
    /// </summary>
    object GetItem(CacheKey cacheKey, string cacheId);
    
    /// <summary>
    /// 整个数据保存到缓存
    /// </summary>
    /// <param name="cacheKey">缓存key</param>
    /// <param name="val">要缓存的数据</param>
    void Set(CacheKey cacheKey, IList val);
    
    /// <summary>
    /// 添加或修改集合的Item
    /// </summary>
    /// <param name="cacheKey">缓存key</param>
    /// <param name="newVal">要缓存的数据</param>
    void SaveItem(CacheKey cacheKey, object newVal);
    
    /// <summary>
    /// 移除缓存
    /// </summary>
    void Remove(CacheKey cacheKey, string cacheId);
    
    /// <summary>
    /// 清空缓存
    /// </summary>
    void Clear(CacheKey cacheKey);

    /// <summary>
    /// 获取集合数量
    /// </summary>
    int Count(CacheKey cacheKey);

    /// <summary>
    /// 指定数据是否存在
    /// </summary>
    bool ExistsItem(CacheKey cacheKey, string cacheId);

    /// <summary>
    /// 缓存是否存在
    /// </summary>
    bool ExistsKey(CacheKey cacheKey);
}