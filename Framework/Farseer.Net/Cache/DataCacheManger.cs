using System;
using System.Collections;

namespace FS.Cache
{
    /// <summary>
    ///     表达式树委托Set缓存
    /// </summary>
    public class DataCacheManger<TValue> : AbsCacheManger<string, TValue> where TValue : class
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        // 不存在数据时，初始化操作
        private readonly Func<TValue> _initCache;

        internal DataCacheManger(string key, Func<TValue> initCache) : base(key: key)
        {
            _initCache = initCache;
        }

        /// <summary>
        ///     当缓存不存在时，上锁加入缓存
        /// </summary>
        protected override TValue SetCacheLock()
        {
            if (_initCache == null) return default;
            lock (LockObject)
            {
                if (!CacheList.ContainsKey(key: Key)) CacheList.Add(key: Key, value: _initCache());
            }

            return CacheList[key: Key];
        }
    }

    /// <summary>
    ///     表达式树委托Set缓存
    /// </summary>
    public class DataCacheManger
    {
        /// <summary>
        ///     清除缓存
        /// </summary>
        public static void Clear()
        {
            DataCacheManger<IList>.Clear();
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key"> 缓存Key </param>
        /// <param name="initCache"> 不存在数据时，初始化操作 </param>
        public static TValue Cache<TValue>(string key, Func<TValue> initCache = null) where TValue : class => new DataCacheManger<TValue>(key: key, initCache: initCache).GetValue();
    }
}