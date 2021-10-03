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
        List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey);
        /// <summary>
        ///     从缓存中读取LIST
        /// </summary>
        Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey);
        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
        /// <summary>
        ///     将LIST保存到缓存中
        /// </summary>
        void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst);
        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity);
        /// <summary>
        ///     将实体保存到缓存中
        /// </summary>
        Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity);
        /// <summary>
        ///     从缓存中读取实体
        /// </summary>
        Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
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
        ///     将LIST保存到缓存中
        /// </summary>
        Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst);
        /// <summary>
        ///     删除整个缓存
        /// </summary>
        void Remove(CacheKey cacheKey);
        /// <summary>
        ///     删除整个缓存
        /// </summary>
        Task RemoveAsync(CacheKey cacheKey);
        /// <summary>
        ///     删除缓存item
        /// </summary>
        void Remove<TEntity,TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
        /// <summary>
        ///     删除缓存item
        /// </summary>
        Task RemoveAsync<TEntity,TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey);
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
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        TEntity Get<TEntity>(CacheKey<TEntity> cacheKey);
        /// <summary>
        ///     从缓存集合中读取实体
        /// </summary>
        /// <param name="cacheKey"> 缓存策略 </param>
        Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey);
    }
}