using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FS.Cache
{
    /// <summary>
    /// 支持缓存不存在，则写入
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        List<TEntity> GetList<TEntity>(string cacheKey, Func<CacheOption, List<TEntity>> get, Func<TEntity, object> getEntityId);

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<CacheOption, List<TEntity>> get, Func<TEntity, object> getEntityId);

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<CacheOption, Task<List<TEntity>>> get, Func<TEntity, object> getEntityId);

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        TEntity ToEntity<TEntity>(string cacheKey, string fieldKey, Func<CacheOption, TEntity> get, Func<TEntity, object> getEntityId);

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="lst">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        void Save<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption =null);

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="entity">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        void Save<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption=null);

        /// <summary>
        /// 删除缓存item
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        /// <param name="fieldKey">缓存Field</param>
        void Remove(string cacheKey, string fieldKey);

        /// <summary>
        /// 删除整个缓存
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        void Remove(string cacheKey);
        
        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        Task<TEntity> ToEntityAsync<TEntity>(string cacheKey, string fieldKey, Func<CacheOption, Task<TEntity>> get, Func<TEntity, object> getEntityId);

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        Task<TEntity> ToEntityAsync<TEntity>(string cacheKey, string fieldKey, Func<CacheOption, TEntity> get, Func<TEntity, object> getEntityId);

        /// <summary>
        /// 删除缓存item
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        /// <param name="fieldKey">缓存Field</param>
        Task RemoveAsync(string cacheKey, string fieldKey);

        /// <summary>
        /// 删除整个缓存
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        Task RemoveAsync(string cacheKey);

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="lst">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        Task SaveAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null);

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="entity">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        Task SaveAsync<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null);

        
    }
}