using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Collections.Pooled;
using StackExchange.Redis;

namespace FS.Cache.Redis;

/// <summary>
///     缓存管理接口
/// </summary>
public interface IRedisCacheManager : IDisposable
{
    /// <summary>
    ///     数据库
    /// </summary>
    IDatabase Db { get; }

    /// <summary>
    ///     支持缓存不存在，则写入
    /// </summary>
    ICacheManager CacheManager { get; }

    /// <summary>
    ///     Redis服务端
    /// </summary>
    string Server { get; }

    /// <summary>
    ///     清除所有缓存
    /// </summary>
    void Clear();

    /// <summary>
    ///     事务，批量写入HASH
    /// </summary>
    void HashSetTransaction<TEntity, TEntityId>(string key, IEnumerable<TEntity> lst, Func<TEntity, TEntityId> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null);

    /// <summary>
    ///     事务，批量写入HASH
    /// </summary>
    Task HashSetTransactionAsync<TEntity, TEntityId>(string key, IEnumerable<TEntity> lst, Func<TEntity, TEntityId> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null);
    
    /// <summary>
    /// 事务锁
    /// </summary>
    /// <param name="key">KEY</param>
    /// <param name="lockTime">锁的时长</param>
    RedisLock GetLocker(string key, TimeSpan lockTime);
    /// <summary>
    /// 从Hash获取集合
    /// </summary>
    IEnumerable<TEntity> HashToList<TEntity>(string key);
    /// <summary>
    /// 从Hash获取集合
    /// </summary>
    IAsyncEnumerable<TEntity> HashToListAsync<TEntity>(string key);
    /// <summary>
    /// 从Hash获取单个实体
    /// </summary>
    TEntity HashToEntity<TEntity>(string key, string field) where TEntity : class;
    /// <summary>
    /// 从Hash获取单个实体
    /// </summary>
    Task<TEntity> HashToEntityAsync<TEntity>(string key, string field) where TEntity : class;
}