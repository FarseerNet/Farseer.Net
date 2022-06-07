using System;
using System.Linq.Expressions;
using FS.Core.Abstract.Cache;

namespace FS.Cache;

/// <summary>
/// 缓存设置
/// </summary>
public interface ICacheServices
{
    /// <summary>
    /// 设置内存缓存（集合）
    /// </summary>
    void SetProfilesInMemory<TEntity, TEntityId>(string key, Expression<Func<TEntity, TEntityId>> getField, TimeSpan? memoryExpiry = null) where TEntity : class;
    /// <summary>
    /// 设置Redis缓存（集合）
    /// </summary>
    void SetProfilesInRedis<TEntity, TEntityId>(string key, string redisConfigName, Expression<Func<TEntity, TEntityId>> getField, TimeSpan? redisExpiry = null) where TEntity : class;
    /// <summary>
    /// 设置内存-Redis缓存（集合）
    /// </summary>
    void SetProfilesInMemoryAndRedis<TEntity, TEntityId>(string key, string redisConfigName, Expression<Func<TEntity, TEntityId>> getField, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) where TEntity : class;
    /// <summary>
    /// 设置内存缓存（缓存单个对象）
    /// </summary>
    void SetProfilesInMemory<TEntity>(string key, TimeSpan? memoryExpiry = null) where TEntity : class;
    /// <summary>
    /// 设置Redis缓存（缓存单个对象）
    /// </summary>
    void SetProfilesInRedis<TEntity>(string key, string redisConfigName, TimeSpan? redisExpiry = null) where TEntity : class;
    /// <summary>
    /// 设置内存-Redis缓存（缓存单个对象）
    /// </summary>
    void SetProfilesInMemoryAndRedis<TEntity>(string key, string redisConfigName, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) where TEntity : class;
}