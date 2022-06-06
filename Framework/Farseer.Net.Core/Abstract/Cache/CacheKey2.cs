using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Collections.Pooled;
using FS.Cache;
using FS.DI;
using FS.Extends;

namespace FS.Core.Abstract.Cache;

/// <summary>
///     读写缓存的选项设置
/// </summary>
public class CacheKey2
{
    public CacheKey2(string key, string redisConfigName, EumCacheStoreType cacheStoreType, Type type = null, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null)
    {
        Key             = key;
        CacheStoreType  = cacheStoreType;
        RedisExpiry     = redisExpiry;
        MemoryExpiry    = memoryExpiry;
        ItemType        = type;
        RedisConfigName = redisConfigName;

        Cache = CacheStoreType switch
        {
            // 内存缓存
            EumCacheStoreType.Memory => IocManager.GetService<ICache>("CacheInMemory"),
            // Redis缓存
            EumCacheStoreType.Redis => IocManager.GetService<ICache>($"CacheInRedis_{RedisConfigName}"),
            _                       => IocManager.GetService<ICache>("CacheInMemory")
        };
    }

    /// <summary>
    /// 缓存KEY
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 缓存的数据Item Type
    /// </summary>
    public Type ItemType { get; set; }

    /// <summary>
    ///     缓存策略（默认Memory模式）
    /// </summary>
    public EumCacheStoreType CacheStoreType { get; set; }

    /// <summary>
    ///     设置Redis缓存过期时间
    /// </summary>
    public TimeSpan? RedisExpiry { get; set; }

    /// <summary>
    ///     设置Memory缓存过期时间
    /// </summary>
    public TimeSpan? MemoryExpiry { get; set; }

    /// <summary>
    /// hash中的主键
    /// </summary>
    public PropertyInfo DataKey { get; set; }

    /// <summary>
    /// Redis配置名称
    /// </summary>
    public string RedisConfigName { get; set; }

    /// <summary>
    /// 缓存的数据Item Type
    /// </summary>
    protected Type ListType { get; set; }

    /// <summary>
    /// 缓存的数据Item Type
    /// </summary>
    protected Type PooledListType { get; set; }

    /// <summary>
    /// 获取缓存实现
    /// </summary>
    public ICache Cache { get; }

    /// <summary>
    /// 返回IList集合
    /// </summary>
    public virtual IList CreateNewList(Type returnType, int count)
    {
        // 返回的类型是List时，则用返回类型，直接创建实例集合
        if (returnType.Namespace == "Collections.Pooled" && returnType.Name == "PooledList`1")
        {
            return (IList)InstanceCacheManger.Cache(PooledListType, count);
        }

        // 返回List
        return (IList)InstanceCacheManger.Cache(ListType, count);
    }

    /// <summary>
    /// 从本地内存中获取
    /// </summary>
    public IList Get() => Cache.Get(this);

    /// <summary>
    /// 从本地内存中获取
    /// </summary>
    public void Set(IList val) => Cache.Set(this, val);

    /// <summary>
    /// 更新缓存
    /// </summary>
    public void Save(object newVal) => Cache.Save(this, newVal);

    /// <summary>
    /// 移除缓存
    /// </summary>
    public void Remove(object cacheId) => Cache.Remove(this, cacheId.ToString());

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void Clear() => Cache.Clear(this);
}

/// <summary>
///     读写缓存的选项设置
/// </summary>
public class CacheKey2<TEntity> : CacheKey2 where TEntity : class
{
    public CacheKey2(string key, string redisConfigName, EumCacheStoreType cacheStoreType, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) : base(key, redisConfigName, cacheStoreType, typeof(TEntity), redisExpiry, memoryExpiry)
    {
        ListType       = typeof(List<TEntity>);
        PooledListType = typeof(PooledList<TEntity>);
    }

    /// <summary>
    /// 返回IList集合
    /// </summary>
    public override IList CreateNewList(Type returnType, int count)
    {
        // 返回的类型是List时，则用返回类型，直接创建实例集合
        if (returnType.Namespace == "Collections.Pooled" && returnType.Name == "PooledList`1")
        {
            return new PooledList<TEntity>(count);
        }

        // 返回List
        return new List<TEntity>(count);
    }
}
/// <summary>
///     读写缓存的选项设置
/// </summary>
public class CacheKey2<TEntity, TEntityId> : CacheKey2<TEntity> where TEntity : class
{
    public CacheKey2(string key, string redisConfigName, Expression<Func<TEntity, TEntityId>> getField, EumCacheStoreType cacheStoreType, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) : base(key, redisConfigName, cacheStoreType, redisExpiry, memoryExpiry)
    {
        DataKey = getField.GetPropertyInfo();
    }
}