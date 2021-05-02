using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    /// <summary>
    /// 缓存管理接口
    /// </summary>
    public interface IRedisCacheManager : IDisposable
    {
        /// <summary>
        ///     数据库
        /// </summary>
        IDatabase Db { get; }

        /// <summary>
        /// 支持缓存不存在，则写入
        /// </summary>
        ICacheManager CacheManager { get; }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        void Clear();

        /// <summary>
        ///     事务，批量写入HASH
        /// </summary>
        void HashSetTransaction<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> funcDataKey, Func<TEntity, string> funcData = null);
    }
}
