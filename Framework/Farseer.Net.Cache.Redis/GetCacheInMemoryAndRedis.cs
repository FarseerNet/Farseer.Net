using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.DI;

namespace FS.Cache.Redis
{
    public class GetCacheInMemoryAndRedis : IGetCache
    {
        private readonly IGetCache _memoryCache;
        private readonly IGetCache _redisCache;

        public GetCacheInMemoryAndRedis(string redisItemConfigName)
        {
            _redisCache  = IocManager.Instance.Resolve<IGetCache>(name: $"GetCacheInRedis_{redisItemConfigName}");
            _memoryCache = IocManager.Instance.Resolve<IGetCache>(name: "GetCacheInMemory");
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public List<TEntity> GetList<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            // 读取memory
            var lst = _memoryCache.GetList(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst is
            {
                Count: > 0
            })
                return lst;

            // 没读到memory，则读redis
            lst = _redisCache.GetList(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst is
            {
                Count: > 0
            })
                _memoryCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);

            return lst;
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public async Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            // 读取memory
            var lst = _memoryCache.GetList(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst is
            {
                Count: > 0
            })
                return lst;

            // 没读到memory，则读redis
            lst = await _redisCache.GetListAsync(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst is
            {
                Count: > 0
            })
            {
                // ReSharper disable once MethodHasAsyncOverload
                _memoryCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
            }

            return lst;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            // 读取memory
            var entity = _memoryCache.GetItem(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (entity != null) return entity;

            // 没读到memory，则读redis list
            var lst = _redisCache.GetList(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst is
            {
                Count: > 0
            })
            {
                // 保存到memory
                _memoryCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey);
            }

            return default;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public async Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            // 读取memory
            var entity = _memoryCache.GetItem(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (entity != null) return entity;

            // 没读到memory，则读redis list
            var lst = await _redisCache.GetListAsync(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst is
            {
                Count: > 0
            })
            {
                // 保存到memory
                // ReSharper disable once MethodHasAsyncOverload
                _memoryCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey);
            }

            return default;
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            _redisCache.SaveItem(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
            _memoryCache.SaveItem(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public async Task SaveItemAsync<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            await _redisCache.SaveItemAsync(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveItem(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            _redisCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public async Task SaveListAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption)
        {
            await _redisCache.SaveListAsync(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove(string cacheKey, string fieldKey)
        {
            _redisCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
            _memoryCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public async Task RemoveAsync(string cacheKey, string fieldKey)
        {
            await _redisCache.RemoveAsync(cacheKey: cacheKey, fieldKey: fieldKey);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public void Remove(string cacheKey)
        {
            _redisCache.Remove(cacheKey: cacheKey);
            _memoryCache.Remove(cacheKey: cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public async Task RemoveAsync(string cacheKey)
        {
            await _redisCache.RemoveAsync(cacheKey: cacheKey);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Remove(cacheKey: cacheKey);
        }


        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public TEntity Get<TEntity>(string cacheKey, CacheOption cacheOption)
        {
            // 先读memory
            var entity = _memoryCache.Get<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);
            if (entity != null) return entity;

            // 读redis
            entity = _redisCache.Get<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);
            if (entity != null) _memoryCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);

            return entity;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(string cacheKey, CacheOption cacheOption)
        {
            // 先读memory
            var entity = _memoryCache.Get<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);
            if (entity != null) return entity;

            // 读redis
            entity = await _redisCache.GetAsync<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);
            if (entity != null)
            {
                // ReSharper disable once MethodHasAsyncOverload
                _memoryCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
            }

            return entity;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public void Save<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            _redisCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task SaveAsync<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            await _redisCache.SaveAsync(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
        }
    }
}