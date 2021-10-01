using System;

namespace FS.Cache
{
    /// <summary>
    ///     读写缓存的选项设置
    /// </summary>
    public class CacheKey
    {
        public CacheKey()
        {
        }

        public CacheKey(string key, EumCacheStoreType cacheStoreType, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null)
        {
            Key            = key;
            CacheStoreType = cacheStoreType;
            RedisExpiry    = redisExpiry;
            MemoryExpiry   = memoryExpiry;
        }
        /// <summary>
        /// 缓存KEY
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     缓存策略（默认Memory模式）
        /// </summary>
        public EumCacheStoreType CacheStoreType { get; set; } = EumCacheStoreType.Redis;

        /// <summary>
        ///     设置Redis缓存过期时间
        /// </summary>
        public TimeSpan? RedisExpiry { get; set; }

        /// <summary>
        ///     设置Memory缓存过期时间
        /// </summary>
        public TimeSpan? MemoryExpiry { get; set; }
    }
}