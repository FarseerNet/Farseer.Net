using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Abstract.Cache;
using FS.DI;

namespace FS.Cache
{
    /// <summary>
    ///     缓存的抽像管理
    ///     支持同时缓存本地 + Redis
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private string _redisItemConfigName;

        public CacheManager() { }

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
        public PooledList<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<IEnumerable<TEntity>> get)
        {
            var getCache = SetCache(cacheKey);

            var lst = getCache.GetList(cacheKey);
            if (lst != null && lst.Count != 0) return lst;
            if (get == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                lst = getCache.GetList(cacheKey);
                if (lst != null && lst.Count != 0) return lst;

                // 根据外部源类型，判断是否需要做类型转换
                lst = PooledList(get);

                if (lst?.Count > 0) getCache.SaveList(cacheKey, lst: lst);
            }
            finally
            {
                slimLock.Release();
            }
            return lst;
        }

        /// <summary>
        /// 根据外部源类型，判断是否需要做类型转换
        /// </summary>
        private PooledList<TEntity> PooledList<TEntity>(Func<IEnumerable<TEntity>> get)
        {
            var lstSource = get();
            if (lstSource == null) return null;

            if (lstSource is PooledList<TEntity> pooledListSource)
            {
                return pooledListSource;
            }
            return lstSource.ToPooledList();
        }

        /// <summary>
        /// 根据外部源类型，判断是否需要做类型转换
        /// </summary>
        private async Task<PooledList<TEntity>> PooledList<TEntity>(Func<Task<IEnumerable<TEntity>>> get)
        {
            var lstSource = await get();
            if (lstSource == null) return null;

            if (lstSource is PooledList<TEntity> pooledListSource)
            {
                return pooledListSource;
            }
            return lstSource.ToPooledList();
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public PooledList<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.GetList(cacheKey);
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<PooledList<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.GetListAsync(cacheKey);
        }

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<PooledList<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<IEnumerable<TEntity>> get)
        {
            var getCache = SetCache(cacheKey);

            var lst = await getCache.GetListAsync(cacheKey);
            if (lst != null && lst.Count != 0) return lst;
            if (get == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                lst = await getCache.GetListAsync(cacheKey);
                if (lst != null && lst.Count != 0) return lst;

                // 根据外部源类型，判断是否需要做类型转换
                lst = PooledList(get);

                if (lst?.Count > 0) await getCache.SaveListAsync(cacheKey, lst: lst);
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
        public async Task<PooledList<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<Task<IEnumerable<TEntity>>> get)
        {
            var getCache = SetCache(cacheKey);
            var lst      = await getCache.GetListAsync(cacheKey);
            if (lst != null && lst.Count != 0) return lst;
            if (get == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                lst = await getCache.GetListAsync(cacheKey);
                if (lst != null && lst.Count != 0) return lst;

                // 根据外部源类型，判断是否需要做类型转换
                lst = await PooledList(get);
                
                if (lst?.Count > 0) await getCache.SaveListAsync(cacheKey, lst: lst);
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
        public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<IEnumerable<TEntity>> get)
        {
            var getCache = SetCache(cacheKey);

            var entity = getCache.GetItem(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                entity = getCache.GetItem(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                // 根据外部源类型，判断是否需要做类型转换
                var lst = PooledList(get);
                if (lst is
                    {
                        Count: > 0
                    })
                {
                    getCache.SaveList(cacheKey, lst: lst);
                    return lst.FirstOrDefault(o => cacheKey.GetField(o).ToString() == fieldKey.ToString());
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
            var getCache = SetCache(cacheKey);
            var entity   = getCache.GetItem(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                entity = getCache.GetItem(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                entity = get();
                getCache.SaveItem(cacheKey, entity);

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
            var getCache = SetCache(cacheKey);
            return getCache.GetItem(cacheKey, fieldKey: fieldKey) ?? default;
        }

        /// <summary>
        ///     从集合中获取其中一项数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<Task<IEnumerable<TEntity>>> get)
        {
            var getCache = SetCache(cacheKey);
            var entity   = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                // 根据外部源类型，判断是否需要做类型转换
                var lst = await PooledList(get);
                
                if (lst is
                    {
                        Count: > 0
                    })
                {
                    await getCache.SaveListAsync(cacheKey, lst: lst);
                    return lst.FirstOrDefault(o => cacheKey.GetField(arg: o).ToString() == fieldKey.ToString());
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
        public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<IEnumerable<TEntity>> get)
        {
            var getCache = SetCache(cacheKey);
            var entity   = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                // 根据外部源类型，判断是否需要做类型转换
                var lst = PooledList(get);
                if (lst is
                    {
                        Count: > 0
                    })
                {
                    await getCache.SaveListAsync(cacheKey, lst: lst);
                    return lst.FirstOrDefault(o => cacheKey.GetField(arg: o).ToString() == fieldKey.ToString());
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
            var getCache = SetCache(cacheKey);
            var entity   = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                entity = get();
                if (entity != null)
                {
                    await getCache.SaveItemAsync(cacheKey, entity: entity);
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
            var getCache = SetCache(cacheKey);
            var entity   = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await getCache.GetItemAsync(cacheKey, fieldKey: fieldKey);
                if (entity != null) return entity;

                entity = await get();
                if (entity != null)
                {
                    await getCache.SaveItemAsync(cacheKey, entity: entity);
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
            var getCache = SetCache(cacheKey);
            return getCache.Exists(cacheKey);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<bool> ExistsAsync(CacheKey cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.ExistsAsync(cacheKey);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public bool ExistsItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.ExistsItem(cacheKey, fieldKey);
        }

        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        public Task<bool> ExistsItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.ExistsItemAsync(cacheKey, fieldKey);
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public long GetCount(CacheKey cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.GetCount(cacheKey);
        }

        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<long> GetCountAsync(CacheKey cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.GetCountAsync(cacheKey);
        }

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, IEnumerable<TEntity> lst)
        {
            var getCache = SetCache(cacheKey);
            getCache.SaveList(cacheKey, lst: lst);
        }

        /// <summary>
        ///     保存集合到集合列表中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, IEnumerable<TEntity> lst)
        {
            var getCache = SetCache(cacheKey);
            return getCache.SaveListAsync(cacheKey, lst: lst);
        }

        /// <summary>
        ///     保存item到集合中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
        {
            var getCache = SetCache(cacheKey);
            getCache.SaveItem(cacheKey, entity: entity);
        }

        /// <summary>
        ///     保存item到集合中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
        {
            var getCache = SetCache(cacheKey);
            return getCache.SaveItemAsync(cacheKey, entity: entity);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void RemoveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            var getCache = SetCache(cacheKey);
            getCache.Remove(cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task RemoveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.RemoveAsync(cacheKey, fieldKey: fieldKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public void Remove(CacheKey cacheKey)
        {
            var getCache = SetCache(cacheKey);
            getCache.Remove(cacheKey);
        }

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task RemoveAsync(CacheKey cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.RemoveAsync(cacheKey);
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public TEntity Get<TEntity>(CacheKey<TEntity> cacheKey, Func<TEntity> get)
        {
            var getCache = SetCache(cacheKey);
            var entity   = getCache.Get(cacheKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            slimLock.Wait();
            try
            {
                entity = getCache.Get(cacheKey);
                if (entity != null) return entity;

                entity = get();
                if (entity != null) getCache.Save(cacheKey, entity: entity);
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
            var getCache = SetCache(cacheKey);
            return getCache.Get(cacheKey);
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey)
        {
            var getCache = SetCache(cacheKey);
            return getCache.GetAsync(cacheKey);
        }

        /// <summary>
        ///     获取缓存（内部不使用集合结构存储），如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public async Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey, Func<TEntity> get)
        {
            var getCache = SetCache(cacheKey);
            var entity   = await getCache.GetAsync(cacheKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await getCache.GetAsync(cacheKey);
                if (entity != null) return entity;

                entity = get();
                if (entity != null) await getCache.SaveAsync(cacheKey, entity: entity);
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
            var getCache = SetCache(cacheKey);
            var entity   = await getCache.GetAsync(cacheKey);
            if (entity != null) return entity;
            if (get    == null) return default;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var slimLock = GetLock(cacheKey);
            await slimLock.WaitAsync();
            try
            {
                entity = await getCache.GetAsync(cacheKey);
                if (entity != null) return entity;

                entity = await get();
                if (entity != null) await getCache.SaveAsync(cacheKey, entity: entity);
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
            var getCache = SetCache(cacheKey);
            getCache.Save(cacheKey, entity: entity);
        }

        /// <summary>
        ///     保存对象（内部不使用集合结构存储）
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        public Task SaveAsync<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
        {
            var getCache = SetCache(cacheKey);
            return getCache.SaveAsync(cacheKey, entity: entity);
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
        private IGetCache SetCache(CacheKey cacheKey)
        {
            switch (cacheKey.CacheStoreType)
            {
                case EumCacheStoreType.Memory:
                    return IocManager.GetService<IGetCache>(name: "GetCacheInMemory");
                case EumCacheStoreType.Redis:
                    return IocManager.GetService<IGetCache>(name: $"GetCacheInRedis_{_redisItemConfigName}");
                case EumCacheStoreType.MemoryAndRedis:
                    return IocManager.GetService<IGetCache>(name: $"GetCacheInMemoryAndRedis_{_redisItemConfigName}");
            }
            throw new Exception($"缓存策略枚举值不正确");
        }

        private static readonly PooledDictionary<string, SemaphoreSlim> DicLock = new();

        private SemaphoreSlim GetLock(CacheKey cacheKey)
        {
            if (DicLock.ContainsKey(cacheKey.Key)) return DicLock[cacheKey.Key];
            return DicLock[cacheKey.Key] = new SemaphoreSlim(1, 1);
        }
    }
}