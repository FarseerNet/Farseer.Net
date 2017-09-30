using System;
using System.Collections;
using System.Collections.Generic;
using Farseer.Net.Cache;
using Farseer.Net.Core.Mapping;
using Farseer.Net.Data.Map;

namespace Farseer.Net.Data.Cache
{
    /// <summary>
    ///     实体类整表缓存
    /// </summary>
    public class EntityCacheManger : AbsCacheManger<SetPhysicsMap, IList>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private EntityCacheManger(SetPhysicsMap key, Func<IList> initCache) : base(key)
        {
            this._initCache = initCache;
        }

        // 不存在数据时，初始化操作
        private readonly Func<IList> _initCache;

        protected override IList SetCacheLock()
        {
            if (_initCache == null) { return null; }
            lock (LockObject) { if (!CacheList.ContainsKey(Key)) { CacheList.Add(Key, _initCache()); } }
            return CacheList[Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="initCache">不存在数据时，初始化操作</param>
        public static List<TEntity> Cache<TEntity>(SetPhysicsMap key, Func<IList> initCache = null)
        {
            return new EntityCacheManger(key, initCache).GetValue() as List<TEntity>;
        }
    }
}