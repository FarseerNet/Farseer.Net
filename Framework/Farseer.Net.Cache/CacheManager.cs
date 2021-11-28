using System;
using System.Collections.Generic;
using System.Threading;
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

        private CacheManager()
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
        /// <param name="cacheKey"> 缓存策略 </param>
        public List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<List<TEntity>> get)
        {
            SetCache(cacheKey);

            var lst = _getCache.GetList(cacheKey);
            if (lst != null && lst.Count != 0) return lst;
            if (get == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                lst = _getCache.GetList(cacheKey);
                if (lst != null && lst.Count != 0) return lst;

                lst = get();
                if (lst?.Count > 0) _getCache.SaveList(cacheKey, lst: lst);
            }
            finally
            {
                slimLock.Release();
            }
            return lst;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.GetList(cacheKey);
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.GetListAsync(cacheKey);
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<List<TEntity>> get)
        {
            SetCache(cacheKey: cacheKey);

            var lst = await _getCache.GetListAsync(cacheKey);
            if (lst != null && lst.Count != 0) return lst;
            if (get == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                lst = await _getCache.GetListAsync(cacheKey);
                if (lst != null && lst.Count != 0) return lst;

                lst = get();
                if (lst?.Count > 0) await _getCache.SaveListAsync(cacheKey, lst: lst);
            }
            finally
            {
                slimLock.Release();
            }
            return lst;
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<Task<List<TEntity>>> get)
        {
            SetCache(cacheKey: cacheKey);
            var lst = await _getCache.GetListAsync(cacheKey);
            if (lst != null && lst.Count != 0) return lst;
            if (get == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                lst = await _getCache.GetListAsync(cacheKey);
                if (lst != null && lst.Count != 0) return lst;

                lst = await get();
                if (lst?.Count > 0) await _getCache.SaveListAsync(cacheKey, lst: lst);
                return lst;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<List<TEntity>> get)
        {
            SetCache(cacheKey: cacheKey);

            var entity = _getCache.GetItem(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                entity = _getCache.GetItem(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                var lst = get();
                if (lst is
                {
                    Count: > 0
                })
                {
                    _getCache.SaveList(cacheKey, lst: lst);
                    return lst.Find(match: o => cacheKey.GetField(o).ToString() == fieldKey.ToString());
                }

                return default;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<TEntity> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = _getCache.GetItem(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                entity = _getCache.GetItem(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                entity = get();
                _getCache.SaveItem(cacheKey, entity);

                return entity;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.GetItem(cacheKey, fieldKey: fieldKey) ?? default;
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<Task<List<TEntity>>> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                var lst = await get();
                if (lst is
                {
                    Count: > 0
                })
                {
                    await _getCache.SaveListAsync(cacheKey, lst: lst);
                    return lst.Find(match: o => cacheKey.GetField(arg: o).ToString() == fieldKey.ToString());
                }

                return default;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<List<TEntity>> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                var lst = get();
                if (lst is
                {
                    Count: > 0
                })
                {
                    await _getCache.SaveListAsync(cacheKey, lst: lst);
                    return lst.Find(match: o => cacheKey.GetField(arg: o).ToString() == fieldKey.ToString());
                }

                return default;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<TEntity> get = null)
        {
            SetCache(cacheKey: cacheKey);
            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                entity = get();
                if (entity != null)
                {
                    await _getCache.SaveItemAsync(cacheKey, entity: entity);
                    return entity;
                }
                return default;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<Task<TEntity>> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await _getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                entity = await get();
                if (entity != null)
                {
                    await _getCache.SaveItemAsync(cacheKey, entity: entity);
                    return entity;
                }

                return default;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public bool Exists(CacheKey cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.Exists(cacheKey);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<bool> ExistsAsync(CacheKey cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.ExistsAsync(cacheKey);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public bool ExistsItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.ExistsItem(cacheKey, fieldKey);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public Task<bool> ExistsItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.ExistsItemAsync(cacheKey, fieldKey);
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public long GetCount(CacheKey cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.GetCount(cacheKey);
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<long> GetCountAsync(CacheKey cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.GetCountAsync(cacheKey);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst)
        {
            SetCache(cacheKey: cacheKey);
            _getCache.SaveList(cacheKey, lst: lst);
        }

        /// <summary>
        ///     保存集合到集合列表中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.SaveListAsync(cacheKey, lst: lst);
        }

        /// <summary>
        ///     保存item到集合中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
        {
            SetCache(cacheKey);
            _getCache.SaveItem(cacheKey, entity: entity);
        }

        /// <summary>
        ///     保存item到集合中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.SaveItemAsync(cacheKey, entity: entity);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void RemoveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            SetCache(cacheKey);
            _getCache.Remove(cacheKey, fieldKey: fieldKey);
        }


        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task RemoveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
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
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey<TEntity> cacheKey, Func<TEntity> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = _getCache.Get(cacheKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                entity = _getCache.Get(cacheKey);
                if (entity != null) return entity;

                entity = get();
                if (entity != null) _getCache.Save(cacheKey, entity: entity);
                return entity;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey<TEntity> cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.Get(cacheKey);
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey)
        {
            SetCache(cacheKey: cacheKey);
            return _getCache.GetAsync(cacheKey);
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey, Func<TEntity> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = await _getCache.GetAsync(cacheKey);
            if (entity != null) return entity;
            if (get    == null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await _getCache.GetAsync(cacheKey);
                if (entity != null) return entity;

                entity = get();
                if (entity != null) await _getCache.SaveAsync(cacheKey, entity: entity);
                return entity;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey, Func<Task<TEntity>> get)
        {
            SetCache(cacheKey: cacheKey);
            var entity = await _getCache.GetAsync<TEntity>(cacheKey);
            if (entity != null) return entity;
            if (get    == null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await _getCache.GetAsync<TEntity>(cacheKey);
                if (entity != null) return entity;

                entity = await get();
                if (entity != null) await _getCache.SaveAsync(cacheKey, entity: entity);
                return entity;
            }
            finally
            {
                slimLock.Release();
            }
        }

        /// <summary>
        ///     保存对象（内部不使用集合结构存储）
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Save<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
        {
            SetCache(cacheKey: cacheKey);
            _getCache.Save(cacheKey, entity: entity);
        }

        /// <summary>
        ///     保存对象（内部不使用集合结构存储）
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
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
                    _getCache = IocManager.GetService<IGetCache>(name: "GetCacheInMemory");
                    break;
                case EumCacheStoreType.Redis:
                    _getCache = IocManager.GetService<IGetCache>(name: $"GetCacheInRedis_{_redisItemConfigName}");
                    break;
                case EumCacheStoreType.MemoryAndRedis:
                    _getCache = IocManager.GetService<IGetCache>(name: $"GetCacheInMemoryAndRedis_{_redisItemConfigName}");
                    break;
            }
        }

        private static readonly Dictionary<string, SemaphoreSlim> DicLock = new();
        private SemaphoreSlim GetLock(CacheKey cacheKey)
        {
            if (DicLock.ContainsKey(cacheKey.Key)) return DicLock[cacheKey.Key];
            return DicLock[cacheKey.Key] = new SemaphoreSlim(1, 1);
        }
    }
}