using System.Collections.Generic;
using System.Threading.Tasks;
using FS.DI;

namespace FS.Cache.Redis;

public class GetCacheInMemoryAndRedis : IGetCache
{
    private readonly IGetCache _memoryCache;
    private readonly IGetCache _redisCache;

    public GetCacheInMemoryAndRedis(string redisItemConfigName)
    {
        _redisCache  = IocManager.GetService<IGetCache>(name: $"GetCacheInRedis_{redisItemConfigName}");
        _memoryCache = IocManager.GetService<IGetCache>(name: "GetCacheInMemory");
    }

    /// <summary>
    ///     从缓存中读取LIST
    /// </summary>
    public List<TEntity> GetList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
    {
        // 读取memory
        var lst = _memoryCache.GetList(cacheKey: cacheKey);
        if (lst is
            {
                Count: > 0
            })
            return lst;

        // 没读到memory，则读redis
        lst = _redisCache.GetList(cacheKey: cacheKey);
        if (lst is
            {
                Count: > 0
            })
            _memoryCache.SaveList(cacheKey: cacheKey, lst: lst);

        return lst;
    }

    /// <summary>
    ///     从缓存中读取LIST
    /// </summary>
    public async Task<List<TEntity>> GetListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey)
    {
        // 读取memory
        var lst = _memoryCache.GetList(cacheKey: cacheKey);
        if (lst is
            {
                Count: > 0
            })
            return lst;

        // 没读到memory，则读redis
        lst = await _redisCache.GetListAsync(cacheKey: cacheKey);
        if (lst is
            {
                Count: > 0
            })
        {
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveList(cacheKey: cacheKey, lst: lst);
        }

        return lst;
    }

    /// <summary>
    ///     从缓存中读取实体
    /// </summary>
    public TEntity GetItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        // 读取memory
        var entity = _memoryCache.GetItem(cacheKey: cacheKey, fieldKey: fieldKey);
        if (entity != null) return entity;

        // 没读到memory，则读redis list
        var lst = _redisCache.GetList(cacheKey: cacheKey);
        if (lst is
            {
                Count: > 0
            })
        {
            // 保存到memory
            _memoryCache.SaveList(cacheKey: cacheKey, lst: lst);
            return lst.Find(match: o => cacheKey.GetField(arg: o).ToString() == fieldKey.ToString());
        }

        return default;
    }

    /// <summary>
    ///     从缓存中读取实体
    /// </summary>
    public async Task<TEntity> GetItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        // 读取memory
        var entity = _memoryCache.GetItem(cacheKey: cacheKey, fieldKey: fieldKey);
        if (entity != null) return entity;

        // 没读到memory，则读redis list
        var lst = await _redisCache.GetListAsync(cacheKey: cacheKey);
        if (lst is
            {
                Count: > 0
            })
        {
            // 保存到memory
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.SaveList(cacheKey: cacheKey, lst: lst);
            return lst.Find(match: o => cacheKey.GetField(arg: o).ToString() == fieldKey.ToString());
        }

        return default;
    }

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public bool Exists(CacheKey cacheKey)
    {
        // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
        var isExists            = _memoryCache.Exists(cacheKey: cacheKey);
        if (!isExists) isExists = _redisCache.Exists(cacheKey: cacheKey);
        return isExists;
    }

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public async Task<bool> ExistsAsync(CacheKey cacheKey)
    {
        // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
        var isExists            = await _memoryCache.ExistsAsync(cacheKey: cacheKey);
        if (!isExists) isExists = await _redisCache.ExistsAsync(cacheKey: cacheKey);
        return isExists;
    }

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
    public bool ExistsItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
        var isExists            = _memoryCache.ExistsItem(cacheKey: cacheKey, fieldKey: fieldKey);
        if (!isExists) isExists = _redisCache.ExistsItem(cacheKey: cacheKey, fieldKey: fieldKey);
        return isExists;
    }

    /// <summary>
    ///     是否存在此项数据
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    /// <param name="fieldKey"> 实体的ID（必须是具有唯一性） </param>
    public async Task<bool> ExistsItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        // 先从本地缓存获取，如果为false，则从redis再次获取（双重验证）
        var isExists            = await _memoryCache.ExistsItemAsync(cacheKey: cacheKey, fieldKey: fieldKey);
        if (!isExists) isExists = await _redisCache.ExistsItemAsync(cacheKey: cacheKey, fieldKey: fieldKey);
        return isExists;
    }

    /// <summary>
    ///     获取集合的数量
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public long GetCount(CacheKey cacheKey) => _memoryCache.GetCount(cacheKey: cacheKey);

    /// <summary>
    ///     获取集合的数量
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public Task<long> GetCountAsync(CacheKey cacheKey) => _memoryCache.GetCountAsync(cacheKey: cacheKey);

    /// <summary>
    ///     将实体保存到缓存中
    /// </summary>
    public void SaveItem<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
    {
        _redisCache.SaveItem(cacheKey: cacheKey, entity: entity);
        _memoryCache.SaveItem(cacheKey: cacheKey, entity: entity);
    }

    /// <summary>
    ///     将实体保存到缓存中
    /// </summary>
    public async Task SaveItemAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntity entity)
    {
        await _redisCache.SaveItemAsync(cacheKey: cacheKey, entity: entity);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.SaveItem(cacheKey: cacheKey, entity: entity);
    }

    /// <summary>
    ///     将LIST保存到缓存中
    /// </summary>
    public void SaveList<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst)
    {
        _redisCache.SaveList(cacheKey: cacheKey, lst: lst);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.SaveList(cacheKey: cacheKey, lst: lst);
    }

    /// <summary>
    ///     将LIST保存到缓存中
    /// </summary>
    public async Task SaveListAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, List<TEntity> lst)
    {
        await _redisCache.SaveListAsync(cacheKey: cacheKey, lst: lst);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.SaveList(cacheKey: cacheKey, lst: lst);
    }

    /// <summary>
    ///     删除整个缓存
    /// </summary>
    public void Remove(CacheKey cacheKey)
    {
        _redisCache.Remove(cacheKey: cacheKey);
        _memoryCache.Remove(cacheKey: cacheKey);
    }

    /// <summary>
    ///     删除整个缓存
    /// </summary>
    public async Task RemoveAsync(CacheKey cacheKey)
    {
        await _redisCache.RemoveAsync(cacheKey: cacheKey);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.Remove(cacheKey: cacheKey);
    }

    /// <summary>
    ///     删除缓存item
    /// </summary>
    public void Remove<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        _redisCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
        _memoryCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
    }

    /// <summary>
    ///     删除缓存item
    /// </summary>
    public async Task RemoveAsync<TEntity, TEntityId>(CacheKey<TEntity, TEntityId> cacheKey, TEntityId fieldKey)
    {
        await _redisCache.RemoveAsync(cacheKey: cacheKey, fieldKey: fieldKey);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.Remove(cacheKey: cacheKey, fieldKey: fieldKey);
    }


    /// <summary>
    ///     从缓存集合中读取实体
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public TEntity Get<TEntity>(CacheKey<TEntity> cacheKey)
    {
        // 先读memory
        var entity = _memoryCache.Get(cacheKey: cacheKey);
        if (entity != null) return entity;

        // 读redis
        entity = _redisCache.Get(cacheKey: cacheKey);
        if (entity != null) _memoryCache.Save(cacheKey: cacheKey, entity: entity);

        return entity;
    }

    /// <summary>
    ///     从缓存集合中读取实体
    /// </summary>
    /// <param name="cacheKey"> 缓存策略 </param>
    public async Task<TEntity> GetAsync<TEntity>(CacheKey<TEntity> cacheKey)
    {
        // 先读memory
        var entity = _memoryCache.Get(cacheKey: cacheKey);
        if (entity != null) return entity;

        // 读redis
        entity = await _redisCache.GetAsync(cacheKey: cacheKey);
        if (entity != null)
        {
            // ReSharper disable once MethodHasAsyncOverload
            _memoryCache.Save(cacheKey: cacheKey, entity: entity);
        }

        return entity;
    }

    /// <summary>
    ///     保存对象
    /// </summary>
    /// <param name="entity"> 保存对象 </param>
    /// <param name="cacheKey"> 缓存策略 </param>
    public void Save<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
    {
        _redisCache.Save(cacheKey: cacheKey, entity: entity);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.Save(cacheKey: cacheKey, entity: entity);
    }

    /// <summary>
    ///     保存对象
    /// </summary>
    /// <param name="entity"> 保存对象 </param>
    /// <param name="cacheKey"> 缓存策略 </param>
    public async Task SaveAsync<TEntity>(CacheKey<TEntity> cacheKey, TEntity entity)
    {
        await _redisCache.SaveAsync(cacheKey: cacheKey, entity: entity);
        // ReSharper disable once MethodHasAsyncOverload
        _memoryCache.Save(cacheKey: cacheKey, entity: entity);
    }
}