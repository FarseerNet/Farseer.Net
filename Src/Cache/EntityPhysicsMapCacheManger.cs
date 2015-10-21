using System;
using FS.Map;

namespace FS.Cache
{
    /// <summary>
    ///     实体类结构映射缓存
    /// </summary>
    public class EntityPhysicsMapCacheManger : AbsCacheManger<Type, EntityPhysicsMap>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private EntityPhysicsMapCacheManger(Type key) : base(key)
        {
        }

        protected override EntityPhysicsMap SetCacheLock()
        {
            lock (LockObject) { if (!CacheList.ContainsKey(Key)) { CacheList.Add(Key, new EntityPhysicsMap(Key)); } }
            return CacheList[Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        public static EntityPhysicsMap Cache(Type key)
        {
            return new EntityPhysicsMapCacheManger(key).GetValue();
        }
    }
}