using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.DI;

namespace FS.Cache
{
    /// <summary>
    ///     缓存的抽像管理
    ///     支持同时缓存本地 + Redis
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private IGetCache _getCache;
        private string    _redisItemConfigName;

        public CacheManager()
        {
        }

        /// <param name="redisItemConfigName"> Redis配置 </param>
        public CacheManager(string redisItemConfigName)
        {
            _redisItemConfigName = redisItemConfigName;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public List<TEntity> GetList<TEntity, TEntityId>(CacheKey cacheKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var lst = _getCache.GetList(cacheKey, getEntityId: getEntityId);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            lst = get();
            if (lst?.Count > 0) _getCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);

            return lst;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var lst = await _getCache.GetListAsync(cacheKey, getEntityId: getEntityId);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            lst = get();
            if (lst?.Count > 0) await _getCache.SaveListAsync(cacheKey, lst: lst, getEntityId: getEntityId);

            return lst;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<Task<List<TEntity>>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var lst = await _getCache.GetListAsync(cacheKey, getEntityId: getEntityId);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            lst = await get();
            if (lst?.Count > 0) await _getCache.SaveListAsync(cacheKey, lst: lst, getEntityId: getEntityId);

            return lst;
        }

        /// <summary>
        ///     从缓存集合中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = _getCache.GetItem(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var lst = get();
            if (lst is
            {
                Count: > 0
            })
            {
                _getCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
                return lst.Find(match: o => getEntityId(o).ToString() == fieldKey.ToString());
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<Task<List<TEntity>>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var lst = await get();
            if (lst is
            {
                Count: > 0
            })
            {
                await _getCache.SaveListAsync(cacheKey, lst: lst, getEntityId: getEntityId);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey.ToString());
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var lst = get();
            if (lst is
            {
                Count: > 0
            })
            {
                await _getCache.SaveListAsync(cacheKey, lst: lst, getEntityId: getEntityId);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey.ToString());
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = get();
            if (entity != null)
            {
                await _getCache.SaveItemAsync(cacheKey, entity: entity, getEntityId: getEntityId);
                return entity;
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<Task<TEntity>> get, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey, getEntityId: getEntityId);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = await get();
            if (entity != null)
            {
                await _getCache.SaveItemAsync(cacheKey, entity: entity, getEntityId: getEntityId);
                return entity;
            }

            return default;
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void SaveList<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            _getCache.SaveList(cacheKey, lst: lst, getEntityId: getEntityId);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveListAsync<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            return _getCache.SaveListAsync(cacheKey, lst: lst, getEntityId: getEntityId);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void SaveItem<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, TEntityId getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            _getCache.SaveItem(cacheKey, entity: entity, getEntityId: o => getEntityId);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, TEntityId getEntityId)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            return _getCache.SaveItemAsync(cacheKey, entity: entity, getEntityId: o => getEntityId);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Remove<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            SetCache(cacheKey);
            _getCache.Remove(cacheKey, fieldKey: fieldKey);
        }


        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task RemoveAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey)
        {
            SetCache(cacheKey);
            return _getCache.RemoveAsync(cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Remove(CacheKey cacheKey)
        {
            SetCache(cacheKey);
            _getCache.Remove(cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task RemoveAsync(CacheKey cacheKey)
        {
            SetCache(cacheKey);
            return _getCache.RemoveAsync(cacheKey);
        }


        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey cacheKey, Func<TEntity> get)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = _getCache.Get<TEntity>(cacheKey);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = get();
            if (entity != null) _getCache.Save(cacheKey, entity: entity);

            return entity;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey, Func<TEntity> get)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = await _getCache.GetAsync<TEntity>(cacheKey);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = get();
            if (entity != null) await _getCache.SaveAsync(cacheKey, entity: entity);

            return entity;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey, Func<Task<TEntity>> get)
        {
            cacheKey ??= new CacheKey();
            SetCache(cacheKey: cacheKey);

            var entity = await _getCache.GetAsync<TEntity>(cacheKey);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = await get();
            if (entity != null) await _getCache.SaveAsync(cacheKey, entity: entity);

            return entity;
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Save<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            SetCache(cacheKey: cacheKey);
            _getCache.Save(cacheKey, entity: entity);
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(CacheKey cacheKey, TEntity entity)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.SaveAsync(cacheKey, entity: entity);
        }

        /// <summary>
        ///     指定哪个配置的Redis
        /// </summary>
        public ICacheManager SetRedisConfigName(string redisItemConfigName)
        {
            _redisItemConfigName = redisItemConfigName;
            return this;
        }

        /// <summary>
        ///     设置缓存策略
        /// </summary>
        private void SetCache(CacheKey cacheKey)
        {
            switch (cacheKey.CacheStoreType)
            {
                case EumCacheStoreType.Memory:
                    _getCache = IocManager.Instance.Resolve<IGetCache>(name: "GetCacheInMemory");
                    break;
                case EumCacheStoreType.Redis:
                    _getCache = IocManager.Instance.Resolve<IGetCache>(name: $"GetCacheInRedis_{_redisItemConfigName}");
                    break;
                case EumCacheStoreType.MemoryAndRedis:
                    _getCache = IocManager.Instance.Resolve<IGetCache>(name: $"GetCacheInMemoryAndRedis_{_redisItemConfigName}");
                    break;
            }
        }
    }
}