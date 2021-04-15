using System;
using FS.Cache;
using FS.ElasticSearch.Map;

namespace FS.ElasticSearch.Cache
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

        private SetMapCacheManger(Type key) : base(key)
        {
        }

        protected override SetPhysicsMap SetCacheLock()
        {
            lock (LockObject) { if (!CacheList.ContainsKey(Key)) { CacheList.Add(Key, new SetPhysicsMap(Key)); } }
            return CacheList[Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        public static SetPhysicsMap Cache(Type key)
        {
            return new SetMapCacheManger(key).GetValue();
        }
    }
}