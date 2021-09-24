using System;
using FS.Cache;
using FS.ElasticSearch.Map;

namespace FS.ElasticSearch.Cache
{
    /// <summary>
    ///     上下文结构映射缓存
    /// </summary>
    public class ContextMapCacheManger : AbsCacheManger<Type, ContextPhysicsMap>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private ContextMapCacheManger(Type key) : base(key: key)
        {
        }

        protected override ContextPhysicsMap SetCacheLock()
        {
            lock (LockObject)
            {
                if (!CacheList.ContainsKey(key: Key)) CacheList.Add(key: Key, value: new ContextPhysicsMap(type: Key));
            }

            return CacheList[key: Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key"> 缓存Key </param>
        public static ContextPhysicsMap Cache(Type key) => new ContextMapCacheManger(key: key).GetValue();
    }
}