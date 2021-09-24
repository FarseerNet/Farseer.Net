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
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        List<TEntity> GetList<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        TEntity GetItem<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="fieldKey"> hash里的field值 </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        /// <param name="getEntityId"> 获取fieldId（用于双缓存时，同步到Memory) </param>
        Task<TEntity> GetItemAsync<TEntity>(string cacheKey, string fieldKey, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        void SaveItem<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="entity"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        Task SaveItemAsync<TEntity>(string cacheKey, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="lst"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        void SaveList<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="lst"> 数据源 </param>
        /// <param name="getEntityId"> 实体的ID（必须是具有唯一性） </param>
        /// <param name="cacheOption"> 缓存配置项 </param>
        Task SaveListAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="fieldKey"> 缓存Field </param>
        void Remove(string cacheKey, string fieldKey);

        /// <summary>
        ///     删除缓存item
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        /// <param name="fieldKey"> 缓存Field </param>
        Task RemoveAsync(string cacheKey, string fieldKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        void Remove(string cacheKey);

        /// <summary>
        ///     删除整个缓存
        /// </summary>
        /// <param name="cacheKey"> 缓存KEY </param>
        Task RemoveAsync(string cacheKey);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        TEntity Get<TEntity>(string cacheKey, CacheOption cacheOption);

        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存Key </param>
        /// <param name="cacheOption"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(string cacheKey, CacheOption cacheOption);

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