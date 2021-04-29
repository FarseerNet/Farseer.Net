using System;
using System.Collections.Generic;

namespace FS.Cache
{
    public interface IGetCache
    {
        /// <summary>
        /// 从缓存中读取LIST
        /// </summary>
        /// <param name="key">缓存Key</param>
        List<TEntity> ToList<TEntity>(string key);

        /// <summary>
        /// 从缓存中读取实体
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        TEntity ToEntity<TEntity>(string key,string fieldKey);

        /// <summary>
        /// 将实体保存到缓存中
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="entity">数据源</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        void Save<TEntity>(string key, TEntity entity, Func<TEntity, object> getEntityId, CacheOption cacheOption);

        /// <summary>
        /// 将LIST保存到缓存中
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="lst">数据源</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        void Save<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption);

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
    }
}