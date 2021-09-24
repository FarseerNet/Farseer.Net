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
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public List<TEntity> GetList<TEntity>(string cacheKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var lst = _getCache.GetList(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            lst = get();
            if (lst?.Count > 0) _getCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);

            return lst;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var lst = await _getCache.GetListAsync(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            lst = get();
            if (lst?.Count > 0) await _getCache.SaveListAsync(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);

            return lst;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<Task<List<TEntity>>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var lst = await _getCache.GetListAsync(cacheKey: cacheKey, getEntityId: getEntityId, cacheOption: cacheOption);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            lst = await get();
            if (lst?.Count > 0) await _getCache.SaveListAsync(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);

            return lst;
        }

        /// <summary>
        ///     从缓存集合中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public TEntity GetItem<TEntity>(string cacheKey, string fieldKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = _getCache.GetItem(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var lst = get();
            if (lst is
            {
                Count: > 0
            })
            {
                _getCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey);
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<Task<List<TEntity>>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = await _getCache.GetItemAsync(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var lst = await get();
            if (lst is
            {
                Count: > 0
            })
            {
                await _getCache.SaveListAsync(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey);
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = await _getCache.GetItemAsync(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var lst = get();
            if (lst is
            {
                Count: > 0
            })
            {
                await _getCache.SaveListAsync(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
                return lst.Find(match: o => getEntityId(arg: o).ToString() == fieldKey);
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<TEntity> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = await _getCache.GetItemAsync(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = get();
            if (entity != null)
            {
                await _getCache.SaveItemAsync(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
                return entity;
            }

            return default;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<Task<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = await _getCache.GetItemAsync(cacheKey: cacheKey, fieldKey: fieldKey, getEntityId: getEntityId, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = await get();
            if (entity != null)
            {
                await _getCache.SaveItemAsync(cacheKey: cacheKey, entity: entity, getEntityId: getEntityId, cacheOption: cacheOption);
                return entity;
            }

            return default;
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        public void SaveList<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            _getCache.SaveList(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        public Task SaveListAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            return _getCache.SaveListAsync(cacheKey: cacheKey, lst: lst, getEntityId: getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        public void SaveItem<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            _getCache.SaveItem(cacheKey: cacheKey, entity: entity, getEntityId: o => getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        public Task SaveItemAsync<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            return _getCache.SaveItemAsync(cacheKey: cacheKey, entity: entity, getEntityId: o => getEntityId, cacheOption: cacheOption);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        public void Remove(string cacheKey, string fieldKey, EumCacheStoreType cacheStoreType)
        {
            SetCache(cacheOption: new CacheOption { CacheStoreType = cacheStoreType });
            _getCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
        }


        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        public Task RemoveAsync(string cacheKey, string fieldKey, EumCacheStoreType cacheStoreType)
        {
            SetCache(cacheOption: new CacheOption { CacheStoreType = cacheStoreType });
            return _getCache.RemoveAsync(cacheKey: cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        public void Remove(string cacheKey, EumCacheStoreType cacheStoreType)
        {
            SetCache(cacheOption: new CacheOption { CacheStoreType = cacheStoreType });
            _getCache.Remove(cacheKey: cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        public Task RemoveAsync(string cacheKey, EumCacheStoreType cacheStoreType)
        {
            SetCache(cacheOption: new CacheOption { CacheStoreType = cacheStoreType });
            return _getCache.RemoveAsync(cacheKey: cacheKey);
        }


        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public TEntity Get<TEntity>(string cacheKey, Func<TEntity> get, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = _getCache.Get<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = get();
            if (entity != null) _getCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);

            return entity;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(string cacheKey, Func<TEntity> get, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = await _getCache.GetAsync<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = get();
            if (entity != null) await _getCache.SaveAsync(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);

            return entity;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(string cacheKey, Func<Task<TEntity>> get, CacheOption cacheOption = null)
        {
            cacheOption ??= new CacheOption();
            SetCache(cacheOption: cacheOption);

            var entity = await _getCache.GetAsync<TEntity>(cacheKey: cacheKey, cacheOption: cacheOption);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            entity = await get();
            if (entity != null) await _getCache.SaveAsync(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);

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
            SetCache(cacheOption: cacheOption);
            _getCache.Save(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
        }

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption)
        {
            SetCache(cacheOption: cacheOption);
            return _getCache.SaveAsync(cacheKey: cacheKey, entity: entity, cacheOption: cacheOption);
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
        private void SetCache(CacheOption cacheOption)
        {
            switch (cacheOption.CacheStoreType)
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