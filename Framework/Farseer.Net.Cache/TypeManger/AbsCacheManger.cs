using System.Collections.Generic;

namespace FS.Cache.TypeManger
{
    /// <summary>
    ///     缓存管理基类
    /// </summary>
    public abstract class AbsCacheManger<TKey, TValue>
    {
        /// <summary>
        ///     缓存Key
        /// </summary>
        protected TKey Key;

        /// <summary>
        ///     缓存类
        /// </summary>
        protected static readonly Dictionary<TKey, TValue> CacheList = new Dictionary<TKey, TValue>();

        /// <summary>
        ///     缓存管理基类
        /// </summary>
        protected AbsCacheManger() { }

        /// <summary>
        ///     缓存管理基类
        /// </summary>
        /// <param name="key">缓存Key</param>
        protected AbsCacheManger(TKey key) { Key = key; }

        /// <summary>
        ///     通过缓存获取数据
        /// </summary>
        public TValue GetValue() => CacheList.ContainsKey(Key) ? CacheList[Key] : SetCacheLock();

        /// <summary>
        ///     更新缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">要更新的值</param>
        public static void Update(TKey key, TValue value) { CacheList[key] = value; }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
        protected abstract TValue SetCacheLock();

        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void Clear() => CacheList.Clear();
    }
}