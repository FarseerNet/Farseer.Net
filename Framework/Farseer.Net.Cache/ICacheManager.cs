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
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<List<TEntity>> get);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<List<TEntity>> get);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, Func<Task<List<TEntity>>> get);
        /// <summary>
        ///     从缓存集合中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<List<TEntity>> get);
        /// <summary>
        ///     从缓存集合中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<Task<List<TEntity>>> get);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<List<TEntity>> get);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<TEntity> get = null);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey, Func<Task<TEntity>> get);
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
        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        bool ExistsItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
        /// <summary>
        /// 是否存在此项数据
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
        Task<bool> ExistsItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
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
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst);
        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst);
        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity);
        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="cacheKey.GetField"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity);
        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        void Remove<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task RemoveAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        void Remove(CacheKey cacheKey);
        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task RemoveAsync(CacheKey cacheKey);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        TEntity Get<TEntity>(CacheKey<TEntity> cacheKey, Func<TEntity> get);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey, Func<TEntity> get);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey, Func<Task<TEntity>> get);
        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        void Save<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity);
        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task SaveAsync<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity);
        /// <summary>
        ///     指定哪个配置的Redis
        /// </summary>
        ICacheManager SetRedisConfigName(string redisItemConfigName);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey);
        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        TEntity Get<TEntity>(CacheKey<TEntity> cacheKey);
    }
}