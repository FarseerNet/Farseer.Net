using System;
using FS.Cache;
using FS.Data.Map;

namespace FS.Data.Cache
{
    /// <summary>
    ///     实体类结构映射缓存
    /// </summary>
    public class SetMapCacheManger : AbsCacheManger<Type, SetPhysicsMap>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private SetMapCacheManger(Type key) : base(key: key)
        {
        }

        protected override SetPhysicsMap SetCacheLock()
        {
            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
            lock (LockObject)
            {
                CacheList.Add(key: Key, value: new SetPhysicsMap(type: Key));
            }

            return CacheList[key: Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key"> 缓存Key </param>
        public static SetPhysicsMap Cache(Type key) => new SetMapCacheManger(key: key).GetValue();
    }
}