using System;
using FS.Cache;
using FS.Data.Map;

namespace FS.Data.Cache
{
    /// <summary>
    ///     上下文结构映射缓存
    /// </summary>
    public class ContextMapCacheManger : AbsCacheManger<Type, ContextPhysicsMap>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private ContextMapCacheManger(Type key) : base(key)
        {
        }

        protected override ContextPhysicsMap SetCacheLock()
        {
            lock (LockObject) { if (!CacheList.ContainsKey(Key)) { CacheList.Add(Key, new ContextPhysicsMap(Key)); } }
            return CacheList[Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        public static ContextPhysicsMap Cache(Type key)
        {
            return new ContextMapCacheManger(key).GetValue();
        }
    }
}