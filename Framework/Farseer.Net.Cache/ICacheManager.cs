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
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        List<TEntity> GetList<TEntity>(string cacheKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<Task<List<TEntity>>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        void SaveList<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        void SaveItem<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        void Remove(string cacheKey, string fieldKey, EumCacheStoreType cacheStoreType);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        void Remove(string cacheKey, EumCacheStoreType cacheStoreType);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        TEntity GetItem<TEntity>(string cacheKey, string fieldKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<Task<List<TEntity>>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<List<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<TEntity> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<Task<TEntity>> get, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="fieldKey"> 缓存Field </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        Task RemoveAsync(string cacheKey, string fieldKey, EumCacheStoreType cacheStoreType);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="cacheStoreType"> 缓存策略 </param>
        Task RemoveAsync(string cacheKey, EumCacheStoreType cacheStoreType);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="lst"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        Task SaveListAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 数据源获取 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        Task SaveItemAsync<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        TEntity Get<TEntity>(string cacheKey, Func<TEntity> get, CacheOption cacheOption = null);

        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(string cacheKey, Func<TEntity> get, CacheOption cacheOption = null);


        /// <summary>
        ///     从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="get"> 数据源获取 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(string cacheKey, Func<Task<TEntity>> get, CacheOption cacheOption = null);

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        void Save<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption);

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task SaveAsync<TEntity>(string cacheKey, TEntity entity, CacheOption cacheOption);
    }
}