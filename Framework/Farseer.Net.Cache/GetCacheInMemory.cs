using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public List<TEntity> GetList<TEntity, TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId)
        {
            if (MyCache.TryGetValue(cacheKey, result: out var result))
            {
                var dic = (ConcurrentDictionary<TEntityId, TEntity>)result;
                return dic.Select(selector: o => o.Value).ToList();
            }

            return null;
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId) => Task.FromResult(result: GetList(cacheKey, getEntityId: getEntityId));

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId)
        {
            if (MyCache.TryGetValue(cacheKey, result: out var result))
            {
                var dic = (ConcurrentDictionary<TEntityId, TEntity>)result;
                dic.TryGetValue(key: fieldKey, value: out var entity);
                return entity;
            }

            return default;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId) => Task.FromResult(result: GetItem(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId));

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public bool Exists(CacheKey cacheKey) => MyCache.TryGetValue(cacheKey, result: out _);

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
        public bool ExistsItem<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            if (MyCache.TryGetValue(cacheKey, result: out var result))
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
        public Task<bool> ExistsItemAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey) => Task.FromResult(result: ExistsItem(cacheKey, fieldKey));

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public long GetCount(CacheKey cacheKey)
        {
            return MyCache.Count;
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<long> GetCountAsync(CacheKey cacheKey) => Task.FromResult(result: GetCount(cacheKey));

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId)
        {
            var fieldKey = getEntityId(arg: entity);
            if (MyCache.TryGetValue(cacheKey, result: out var result))
            {
                var dic = (ConcurrentDictionary<TEntityId, TEntity>)result;
                dic.TryAdd(fieldKey, entity);
                return;
            }

            var cacheEntry = MyCache.CreateEntry(cacheKey);

            // 设置过期时间
            if (cacheKey.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheKey.MemoryExpiry.GetValueOrDefault();
            var newDic                                                                    = new ConcurrentDictionary<TEntityId, TEntity>();
            newDic.TryAdd(fieldKey, entity);
            cacheEntry.Value = newDic;
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public Task SaveItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId)
        {
            SaveItem(cacheKey, entity: entity, getEntityId: getEntityId);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            var dic        = new ConcurrentDictionary<TEntityId, TEntity>();
            var cacheEntry = MyCache.CreateEntry(cacheKey);

            // 设置过期时间
            if (cacheKey.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheKey.MemoryExpiry.GetValueOrDefault();

            foreach (var entity in lst)
            {
                var fieldKey = getEntityId(arg: entity);
                dic.TryAdd(key: fieldKey, value: entity);
            }

            cacheEntry.Value = dic;
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public Task SaveListAsync<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            if (MyCache.TryGetValue(cacheKey, result: out var result))
            {
                var dic = (IDictionary)result;
                dic.Remove(key: fieldKey);
            }
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public Task RemoveAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            Remove(cacheKey, fieldKey: fieldKey);
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
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey cacheKey)
        {
            if (MyCache.TryGetValue(cacheKey, result: out var result))
            {
                if (result != null) return (TEntity)result;
            }

            return default;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey) => Task.FromResult(result: Get<TEntity>(cacheKey));

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Save<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            var cacheEntry = MyCache.CreateEntry(cacheKey);

            // 设置过期时间
            if (cacheKey.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheKey.MemoryExpiry.GetValueOrDefault();

            cacheEntry.Value = entity;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            Save(cacheKey, entity: entity);
            return Task.FromResult(result: 0);
        }
    }
}