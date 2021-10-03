using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FS.Cache
{
    public interface IGetCache
    {
        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        List<TEntity> GetList<TEntity,TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity,TEntityId>(CacheKey cacheKey, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        TEntity GetItem<TEntity,TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        Task<TEntity> GetItemAsync<TEntity,TEntityId>(CacheKey cacheKey, TEntityId fieldKey, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        /// <param name="entity"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存配置项 </param>
        void SaveItem<TEntity,TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        /// <param name="entity"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存配置项 </param>
        Task SaveItemAsync<TEntity,TEntityId>(CacheKey cacheKey, TEntity entity, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        /// <param name="lst"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存配置项 </param>
        void SaveList<TEntity,TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        /// <param name="lst"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheKey"> 缓存配置项 </param>
        Task SaveListAsync<TEntity,TEntityId>(CacheKey cacheKey, List<TEntity> lst, Func<TEntity, TEntityId> getEntityId);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        void Remove<TEntityId>(CacheKey cacheKey, TEntityId fieldKey);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="fieldKey"> 缓存Field </param>
        Task RemoveAsync<TEntityId>(CacheKey cacheKey, TEntityId fieldKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        void Remove(CacheKey cacheKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        Task RemoveAsync(CacheKey cacheKey);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        TEntity Get<TEntity>(CacheKey cacheKey);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey cacheKey);

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
        void Save<TEntity>(CacheKey cacheKey, TEntity entity);

        /// <summary>
        ///     保存对象
        /// </summary>
        /// <param name="entity"> 保存对象 </param>
        /// <param name="cacheKey"> 缓存策略 </param>
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