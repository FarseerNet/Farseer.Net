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
public class CacheKey
{
    public CacheKey(string key, string redisConfigName, EumCacheStoreType cacheStoreType, Type type = null, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null)
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
            _                       => IocManager.GetService<ICache>($"CacheInMemoryAndRedis_{RedisConfigName}")
        };
    }

    /// <summary>
    /// 缓存KEY
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 缓存的数据Item Type
    /// </summary>
    public Type ItemType { get; }

    /// <summary>
    ///     缓存策略（默认Memory模式）
    /// </summary>
    public EumCacheStoreType CacheStoreType { get; }

    /// <summary>
    ///     设置Redis缓存过期时间
    /// </summary>
    public TimeSpan? RedisExpiry { get; }

    /// <summary>
    ///     设置Memory缓存过期时间
    /// </summary>
    public TimeSpan? MemoryExpiry { get; }

    /// <summary>
    /// hash中的主键
    /// </summary>
    public PropertyInfo DataKey { get; protected set; }

    /// <summary>
    /// Redis配置名称
    /// </summary>
    public string RedisConfigName { get; }

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
    public void Set(object val, Type returnType)
    {
        IList list;
        
        if (val is IEnumerable enumerable) // IEnumerable类型，则自己创建一个IList
        {
            if (DataKey == null) throw new System.Exception($"缓存集合时，需设置每个item的唯一标识：ICacheServices.SetProfilesXXXXX(key, getField)");
            list = CreateNewList(returnType, 100);
            foreach (var item in enumerable)
            {
                list.Add(item);
            }
        }
        else // 原来不是集合类型，则将其作为item添加到新的集合
        {
            list = CreateNewList(returnType, 1);
            list.Add(val);
        }

        Cache.Set(this, list);
    }

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
public class CacheKey<TEntity> : CacheKey where TEntity : class
{
    public CacheKey(string key, string redisConfigName, EumCacheStoreType cacheStoreType, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) : base(key, redisConfigName, cacheStoreType, typeof(TEntity), redisExpiry, memoryExpiry)
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
public class CacheKey<TEntity, TEntityId> : CacheKey<TEntity> where TEntity : class
{
    public CacheKey(string key, string redisConfigName, Expression<Func<TEntity, TEntityId>> getField, EumCacheStoreType cacheStoreType, TimeSpan? redisExpiry = null, TimeSpan? memoryExpiry = null) : base(key, redisConfigName, cacheStoreType, redisExpiry, memoryExpiry)
    {
        DataKey = getField.GetPropertyInfo();
    }
}