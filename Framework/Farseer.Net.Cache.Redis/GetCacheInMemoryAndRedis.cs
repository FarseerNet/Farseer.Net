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
        public List<TEntity> GetList<TEntity, TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId)
        {
            // 读取memory
            var lst = _memoryCache.GetList(cacheKey, getEntityId: getEntityId);
            if (lst is
            {
                Count: > 0
            })
                return lst;

            // 没读到memory，则读redis
            lst = _redisCache.GetList(cacheKey, getEntityId: getEntityId);
            if (lst is
            {
                Count: > 0
            })
                _memoryCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);

            return lst;
        }

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId)
        {
            // 读取memory
            var lst = _memoryCache.GetList(cacheKey, getEntityId: getEntityId);
            if (lst is
            {
                Count: > 0
            })
                return lst;

            // 没读到memory，则读redis
            lst = await _redisCache.GetListAsync(cacheKey, getEntityId: getEntityId);
            if (lst is
            {
                Count: > 0
            })
            {
                // ReSharper disable once MethodHasAsyncOverload
                _memoryCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
            }

            return lst;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId)
        {
            // 读取memory
            var entity = _memoryCache.GetItem(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);
            if (entity != null) return entity;

            // 没读到memory，则读redis list
            var lst = _redisCache.GetList(cacheKey, getEntityId: getEntityId);
            if (lst is
            {
                Count: > 0
            })
            {
                // 保存到memory
                _memoryCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey.ToString());
            }

            return default;
        }

        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId)
        {
            // 读取memory
            var entity = _memoryCache.GetItem(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);
            if (entity != null) return entity;

            // 没读到memory，则读redis list
            var lst = await _redisCache.GetListAsync(cacheKey, getEntityId: getEntityId);
            if (lst is
            {
                Count: > 0
            })
            {
                // 保存到memory
                // ReSharper disable once MethodHasAsyncOverload
                _memoryCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey.ToString());
            }

            return default;
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public bool Exists(CacheKey cacheKey)
        {
            // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
            var isExists            = _memoryCache.Exists(cacheKey);
            if (!isExists) isExists = _redisCache.Exists(cacheKey);
            return isExists;
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<bool> ExistsAsync(CacheKey cacheKey)
        {
            // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
            var isExists            = await _memoryCache.ExistsAsync(cacheKey);
            if (!isExists) isExists = await _redisCache.ExistsAsync(cacheKey);
            return isExists;
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public bool ExistsItem<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
            var isExists            = _memoryCache.ExistsItem(cacheKey, fieldKey);
            if (!isExists) isExists = _redisCache.ExistsItem(cacheKey, fieldKey);
            return isExists;
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public async Task<bool> ExistsItemAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
            var isExists            = await _memoryCache.ExistsItemAsync(cacheKey, fieldKey);
            if (!isExists) isExists = await _redisCache.ExistsItemAsync(cacheKey, fieldKey);
            return isExists;
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public long GetCount(CacheKey cacheKey) => _memoryCache.GetCount(cacheKey);

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<long> GetCountAsync(CacheKey cacheKey) => _memoryCache.GetCountAsync(cacheKey);

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public void SaveItem<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId)
        {
            _redisCache.SaveItem(cacheKey, entity: entity, getEntityId: getEntityId);
            _memoryCache.SaveItem(cacheKey, entity: entity, getEntityId: getEntityId);
        }

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        public async Task SaveItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId)
        {
            await _redisCache.SaveItemAsync(cacheKey, entity: entity, getEntityId: getEntityId);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveItem(cacheKey, entity: entity, getEntityId: getEntityId);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public void SaveList<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            _redisCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
        }

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        public async Task SaveListAsync<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            await _redisCache.SaveListAsync(cacheKey, lst: lst, getEntityId: getEntityId);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public void Remove<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            _redisCache.Remove(cacheKey, fieldKey: fieldKey);
            _memoryCache.Remove(cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        public async Task RemoveAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            await _redisCache.RemoveAsync(cacheKey, fieldKey: fieldKey);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Remove(cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public void Remove(CacheKey cacheKey)
        {
            _redisCache.Remove(cacheKey);
            _memoryCache.Remove(cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        public async Task RemoveAsync(CacheKey cacheKey)
        {
            await _redisCache.RemoveAsync(cacheKey);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Remove(cacheKey);
        }


        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey cacheKey)
        {
            // 先读memory
            var entity = _memoryCache.Get<TEntity>(cacheKey);
            if (entity != null) return entity;

            // 读redis
            entity = _redisCache.Get<TEntity>(cacheKey);
            if (entity != null) _memoryCache.Save(cacheKey, entity: entity);

            return entity;
        }

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey)
        {
            // 先读memory
            var entity = _memoryCache.Get<TEntity>(cacheKey);
            if (entity != null) return entity;

            // 读redis
            entity = await _redisCache.GetAsync<TEntity>(cacheKey);
            if (entity != null)
            {
                // ReSharper disable once MethodHasAsyncOverload
                _memoryCache.Save(cacheKey, entity: entity);
            }

            return entity;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Save<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            _redisCache.Save(cacheKey, entity: entity);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Save(cacheKey, entity: entity);
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task SaveAsync<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            await _redisCache.SaveAsync(cacheKey, entity: entity);
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Save(cacheKey, entity: entity);
        }
    }
}