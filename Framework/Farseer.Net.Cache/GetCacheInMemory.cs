using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Abstract.Cache;
using FS.Extends;
using Microsoft.Extensions.Caching.Memory;

namespace FS.Cache
{
    /// <summary>
    ///     使用本地缓存MemoryCache
    /// </summary>
    public class GetCacheInMemory : IGetCache
    {
        private static readonly MemoryCache MyCache = new(optionsAccessor: new MemoryCacheOptions());

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public PooledList<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
        {
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                var dic = (ConcurrentDictionary<TEntityId, TEntity>)result;
                return dic.Select(selector: o => o.Value).ToPooledList().Clone();
            }

            return null;
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public Task<PooledList<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey) => Task.FromResult(result: GetList(cacheKey));

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                var dic = (ConcurrentDictionary<TEntityId, TEntity>)result;
                dic.TryGetValue(key: fieldKey, value: out var entity);
                return entity.Clone();
            }

            return default;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey) => Task.FromResult(result: GetItem(cacheKey, fieldKey: fieldKey));

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public bool Exists(CacheKey cacheKey) => MyCache.TryGetValue(cacheKey.Key, result: out _);

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<bool> ExistsAsync(CacheKey cacheKey) => Task.FromResult(Exists(cacheKey));

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public bool ExistsItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                var dic = (IDictionary)result;
                return dic.Contains(fieldKey);
            }

            return false;
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public Task<bool> ExistsItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey) => Task.FromResult(result: ExistsItem(cacheKey, fieldKey));

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public long GetCount(CacheKey cacheKey)
        {
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                var dic = (IDictionary)result;
                return dic.Count;
            }
            return 0;
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<long> GetCountAsync(CacheKey cacheKey) => Task.FromResult(result: GetCount(cacheKey));

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
        {
            var fieldKey = cacheKey.GetField(arg: entity);
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                var dic = (ConcurrentDictionary<TEntityId, TEntity>)result;
                dic.TryAdd(fieldKey, entity);
                return;
            }

            SaveList(cacheKey, new PooledList<TEntity>() { entity });
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
        {
            SaveItem(cacheKey, entity: entity);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, IEnumerable<TEntity> lst)
        {
            var dic = new ConcurrentDictionary<TEntityId, TEntity>();
            foreach (var entity in lst)
            {
                var fieldKey = cacheKey.GetField(arg: entity);
                dic.TryAdd(key: fieldKey, value: entity);
            }

            using (var cacheEntry = MyCache.CreateEntry(cacheKey.Key))
            {
                // 设置过期时间
                if (cacheKey.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheKey.MemoryExpiry.GetValueOrDefault();
                cacheEntry.Value = dic;
            }
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, IEnumerable<TEntity> lst)
        {
            SaveList(cacheKey, lst: lst);
            return Task.FromResult(result: 0);
        }
        
        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public void Remove(CacheKey cacheKey)
        {
            MyCache.Remove(cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public Task RemoveAsync(CacheKey cacheKey)
        {
            Remove(cacheKey);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                var dic = (IDictionary)result;
                dic.Remove(key: fieldKey);
            }
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public Task RemoveAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            Remove(cacheKey, fieldKey: fieldKey);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey<TEntity> cacheKey)
        {
            if (MyCache.TryGetValue(cacheKey.Key, result: out var result))
            {
                if (result != null) return ((TEntity)result).Clone();
            }

            return default;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey) => Task.FromResult(result: Get(cacheKey));

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Save<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
        {
            using (var cacheEntry = MyCache.CreateEntry(cacheKey.Key))
            {
                // 设置过期时间
                if (cacheKey.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheKey.MemoryExpiry.GetValueOrDefault();
                cacheEntry.Value = entity;
            }
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
        {
            Save(cacheKey, entity: entity);
            return Task.FromResult(result: 0);
        }
    }
}