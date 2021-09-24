using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
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
        void HashSetTransaction<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null);

        /// <summary>
        ///     事务，批量写入HASH
        /// </summary>
        Task HashSetTransactionAsync<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null);
    }
}