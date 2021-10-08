using System;
using System.Collections;
using System.Collections.Generic;
using FS.Cache;
using FS.Data.Map;

namespace FS.Data.Cache
{
    /// <summary>
    ///     实体类整表缓存
    /// </summary>
    public class EntityCacheManger : AbsCacheManger<SetPhysicsMap, IList>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        // 不存在数据时，初始化操作
        private readonly Func<IList> _initCache;

        private EntityCacheManger(SetPhysicsMap key, Func<IList> initCache) : base(key: key)
        {
            _initCache = initCache;
        }

        protected override IList SetCacheLock()
        {
            if (_initCache == null) return null;

            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
            lock (LockObject)
            {
                CacheList.Add(key: Key, value: _initCache());
            }

            return CacheList[key: Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key"> 缓存Key </param>
        /// <param name="initCache"> 不存在数据时，初始化操作 </param>
        public static List<TEntity> Cache<TEntity>(SetPhysicsMap key, Func<IList> initCache = null) => new EntityCacheManger(key: key, initCache: initCache).GetValue() as List<TEntity>;
    }
}