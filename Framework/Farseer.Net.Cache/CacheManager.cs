using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FS.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly IGetCache _getCache;

        public CacheManager(IGetCache getCache)
        {
            _getCache = getCache;
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        public List<TEntity> GetList<TEntity>(string cacheKey, Func<CacheOption, List<TEntity>> get, Func<TEntity, object> getEntityId)
        {
            var lst = _getCache.ToList<TEntity>(cacheKey);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var cacheOption = new CacheOption();
            lst = get(cacheOption);
            if (lst?.Count > 0) _getCache.Save(cacheKey, lst, getEntityId, cacheOption);

            return lst;
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        public async Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<CacheOption, List<TEntity>> get, Func<TEntity, object> getEntityId)
        {
            var lst = await _getCache.ToListAsync<TEntity>(cacheKey);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var cacheOption = new CacheOption();
            lst = get(cacheOption);
            if (lst?.Count > 0) await _getCache.SaveAsync(cacheKey, lst, getEntityId, cacheOption);

            return lst;
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        public async Task<List<TEntity>> GetListAsync<TEntity>(string cacheKey, Func<CacheOption, Task<List<TEntity>>> get, Func<TEntity, object> getEntityId)
        {
            var lst = await _getCache.ToListAsync<TEntity>(cacheKey);
            if (lst != null && lst.Count != 0) return lst;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var cacheOption = new CacheOption();
            lst = await get(cacheOption);
            if (lst?.Count > 0) await _getCache.SaveAsync(cacheKey, lst, getEntityId, cacheOption);

            return lst;
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        public TEntity ToEntity<TEntity>(string cacheKey, string fieldKey, Func<CacheOption, TEntity> get, Func<TEntity, object> getEntityId)
        {
            var entity = _getCache.ToEntity<TEntity>(cacheKey, fieldKey);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var cacheOption = new CacheOption();
            entity = get(cacheOption);
            if (entity != null) _getCache.Save(cacheKey, entity, getEntityId, cacheOption);

            return entity;
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        public async Task<TEntity> ToEntityAsync<TEntity>(string cacheKey, string fieldKey, Func<CacheOption, Task<TEntity>> get, Func<TEntity, object> getEntityId)
        {
            var entity = await _getCache.ToEntityAsync<TEntity>(cacheKey, fieldKey);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var cacheOption = new CacheOption();
            entity = await get(cacheOption);
            if (entity != null) await _getCache.SaveAsync(cacheKey, entity, getEntityId, cacheOption);

            return entity;
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在则通过get委托获取，并保存到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="fieldKey">hash里的field值 </param>
        /// <param name="get">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        public async Task<TEntity> ToEntityAsync<TEntity>(string cacheKey, string fieldKey, Func<CacheOption, TEntity> get, Func<TEntity, object> getEntityId)
        {
            var entity = await _getCache.ToEntityAsync<TEntity>(cacheKey, fieldKey);

            if (entity != null) return entity;

            // 缓存没有，则通过get委托获取（一般是数据库的数据源）
            var cacheOption = new CacheOption();
            entity = get(cacheOption);
            if (entity != null) await _getCache.SaveAsync(cacheKey, entity, getEntityId, cacheOption);

            return entity;
        }

        /// <summary>
        /// 删除缓存item
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        /// <param name="fieldKey">缓存Field</param>
        public void Remove(string cacheKey, string fieldKey) => _getCache.Remove(cacheKey, fieldKey);


        /// <summary>
        /// 删除缓存item
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        /// <param name="fieldKey">缓存Field</param>
        public Task RemoveAsync(string cacheKey, string fieldKey) => _getCache.RemoveAsync(cacheKey, fieldKey);

        /// <summary>
        /// 删除整个缓存
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        public void Remove(string cacheKey) => _getCache.Remove(cacheKey);

        /// <summary>
        /// 删除整个缓存
        /// </summary>
        /// <param name="cacheKey">缓存KEY</param>
        public Task RemoveAsync(string cacheKey) => _getCache.RemoveAsync(cacheKey);

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="lst">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public void Save<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            _getCache.Save(cacheKey, lst, getEntityId, cacheOption);
        }

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="lst">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public Task SaveAsync<TEntity>(string cacheKey, List<TEntity> lst, Func<TEntity, object> getEntityId, CacheOption cacheOption = null)
        {
            return _getCache.SaveAsync(cacheKey, lst, getEntityId, cacheOption);
        }

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="entity">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public void Save<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null)
        {
            _getCache.Save(cacheKey, entity, o => getEntityId, cacheOption);
        }

        /// <summary>
        /// 保存列表到缓存中
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="entity">数据源获取</param>
        /// <param name="getEntityId">实体的ID（必须是具有唯一性）</param>
        /// <param name="cacheOption">缓存配置项 </param>
        public Task SaveAsync<TEntity>(string cacheKey, TEntity entity, object getEntityId, CacheOption cacheOption = null)
        {
            return _getCache.SaveAsync(cacheKey, entity, o => getEntityId, cacheOption);
        }
    }
}