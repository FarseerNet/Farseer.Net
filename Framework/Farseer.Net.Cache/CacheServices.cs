using System;
using System.Linq.Expressions;
using FS.Core.Abstract.Cache;

namespace FS.Cache;

/// <summary>
/// 缓存设置
/// </summary>
public class CacheServices : ICacheServices
{
    /// <summary>
    /// 设置内存缓存（集合）
    /// </summary>
    public void SetProfilesInMemory<TEntity, TEntityId>(string key, Expression<Func<TEntity, TEntityId>> getField, TimeSpan? memoryExpiry = null) where TEntity : class
    {
        CacheConfigure.Configure[key] = new CacheKey2<TEntity, TEntityId>(key, null, getField, EumCacheStoreType.Memory, null, memoryExpiry);
    }

    /// <summary>
    /// 设置Redis缓存（集合）
    /// </summary>
    public void SetProfilesInRedis<TEntity, TEntityId>(string key, string redisConfigName, Expression<Func<TEntity, TEntityId>> getField, TimeSpan? redisExpiry = null) where TEntity : class
    {
        CacheConfigure.Configure[key] = new CacheKey2<TEntity, TEntityId>(key, redisConfigName, getField, EumCacheStoreType.Redis, redisExpiry);
    }

    /// <summary>
    /// 设置内存-Redis缓存（集合）
    /// </summary>
    public void SetProfilesInMemoryAndRedis<TEntity, TEntityId>(string key, string redisConfigName, Expression<Func<TEntity, TEntityId>> getField, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) where TEntity : class
    {
        CacheConfigure.Configure[key] = new CacheKey2<TEntity, TEntityId>(key, redisConfigName, getField, EumCacheStoreType.MemoryAndRedis, redisExpiry, memoryExpiry);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    public void SetProfiles(string key, EumCacheStoreType cacheStoreType, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null)
    {
        CacheConfigure.Configure[key] = new CacheKey2(key, null, cacheStoreType, redisExpiry: redisExpiry, memoryExpiry: memoryExpiry);
    }
}