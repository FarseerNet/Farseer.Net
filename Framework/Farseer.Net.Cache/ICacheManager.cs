using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FS.Cache
{
    /// <summary>
    ///     支持缓存不存在，则写入
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        List<TEntity> GetList<TEntity, TEntityId>(CacheKey cacheKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey cacheKey, Func<Task<List<TEntity>>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        void SaveList<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        void SaveItem<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, TEntityId getEntityId);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 缓存Field </param>
        void Remove<TEntityId>(CacheKey cacheKey, TEntityId fieldKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        void Remove(CacheKey cacheKey);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        TEntity GetItem<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<Task<List<TEntity>>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<List<TEntity>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<Task<TEntity>> get, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 缓存Field </param>
        Task RemoveAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        Task RemoveAsync(CacheKey cacheKey);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task SaveListAsync<TEntity, TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        Task SaveItemAsync<TEntity, TEntityId>(CacheKey cacheKey, TEntity entity, TEntityId getEntityId);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="get"> 数据源获取 </param>
        TEntity Get<TEntity>(CacheKey cacheKey, Func<TEntity> get = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="get"> 数据源获取 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey, Func<TEntity> get = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="get"> 数据源获取 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey, Func<Task<TEntity>> get = null);

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="entity"> 保存对象 </param>
        void Save<TEntity>(CacheKey cacheKey, TEntity entity);

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="entity"> 保存对象 </param>
        Task SaveAsync<TEntity>(CacheKey cacheKey, TEntity entity);
        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        bool ExistsItem<TEntityId>(CacheKey cacheKey, TEntityId fieldKey);
        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        Task<bool> ExistsItemAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey);
        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        long GetCount(CacheKey cacheKey);
        /// <summary>
        /// 获取集合的数量
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<long> GetCountAsync(CacheKey cacheKey);
        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        bool Exists(CacheKey cacheKey);
        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<bool> ExistsAsync(CacheKey cacheKey);
    }
}