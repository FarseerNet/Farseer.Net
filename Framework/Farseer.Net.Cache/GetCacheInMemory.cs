using System;
using System.Collections;
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
        public List<TEntity> GetList<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            if (MyCache.TryGetValue(key: cacheKey, result: out var result))
            {
                var dic = (Dictionary<string, TEntity>)result;
                return dic.Select(selector: o => o.Value).ToList();
            }

            return null;
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption) => Task.FromResult(result: GetList(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption));

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            if (MyCache.TryGetValue(key: cacheKey, result: out var result))
            {
                var dic = (Dictionary<string, TEntity>)result;
                dic.TryGetValue(key: fieldKey, value: out var entity);
                return entity;
            }

            return default;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption) => Task.FromResult(result: GetItem(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption));

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var fieldKey = getEntityId(arg: entity).ToString();
            if (MyCache.TryGetValue(key: cacheKey, result: out var result))
            {
                var dic = (Dictionary<string, TEntity>)result;
                dic.Add(key: fieldKey, value: entity);
                return;
            }

            var cacheEntry = MyCache.CreateEntry(key: cacheKey);

            // 设置过期时间
            if (cacheOption.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheOption.MemoryExpiry.GetValueOrDefault();
            cacheEntry.Value = new Dictionary<string, TEntity> { { fieldKey, entity } };
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public Task SaveItemAsync<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            SaveItem(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            var dic        = new Dictionary<string, TEntity>();
            var cacheEntry = MyCache.CreateEntry(key: cacheKey);

            // 设置过期时间
            if (cacheOption.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheOption.MemoryExpiry.GetValueOrDefault();

            foreach (var entity in lst)
            {
                var fieldKey = getEntityId(arg: entity).ToString();
                dic.Add(key: fieldKey, value: entity);
            }

            cacheEntry.Value = dic;
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public Task SaveListAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove(string cacheKey, string fieldKey)
        {
            if (MyCache.TryGetValue(key: cacheKey, result: out var result))
            {
                var dic = (IDictionary)result;
                dic.Remove(key: fieldKey);
            }
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public Task RemoveAsync(string cacheKey, string fieldKey)
        {
            Remove(cacheKey: cacheKey, fieldKey: fieldKey);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public void Remove(string cacheKey)
        {
            MyCache.Remove(key: cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public Task RemoveAsync(string cacheKey)
        {
            Remove(cacheKey: cacheKey);
            return Task.FromResult(result: 0);
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public TEntity Get<TEntity>(string cacheKey, CacheOption cacheOption)
        {
            if (MyCache.TryGetValue(key: cacheKey, result: out var result))
            {
                if (result != null) return (TEntity)result;
            }

            return default;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public Task<TEntity> GetAsync<TEntity>(string cacheKey, CacheOption cacheOption) => Task.FromResult(result: Get<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption));

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public void Save<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            var cacheEntry = MyCache.CreateEntry(key: cacheKey);

            // 设置过期时间
            if (cacheOption.MemoryExpiry != null) cacheEntry.AbsoluteExpirationRelativeToNow = cacheOption.MemoryExpiry.GetValueOrDefault();

            cacheEntry.Value = entity;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
            return Task.FromResult(result: 0);
        }
    }
}