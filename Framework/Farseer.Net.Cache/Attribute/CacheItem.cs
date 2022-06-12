using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.Abstract.Cache;
using FS.Extends;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Cache.Attribute;

/// <summary>
/// 缓存读取集合中的元素
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class CacheItem : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private string _key;
    public CacheItem(string key)
    {
        _key = key;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var returnType = ((MethodInfo)args.Method).ReturnType;
        var cacheKey   = CacheConfigure.Get(_key);
        var cacheId = args.Arguments.Count switch
        {
            1   => args.Arguments[0],
            > 1 => args.GetParamValueByAttribute<CacheIdAttribute>(),
            _   => null
        };

        if (cacheId           == null) throw new FarseerException($"未找到cacheId");
        if (cacheId.GetType() != cacheKey.DataKey.PropertyType) throw new FarseerException($"cacheId的类型：{cacheId.GetType().Name}与设置的类型：{cacheKey.DataKey.PropertyType.Name} 不一致");
        if (returnType        != cacheKey.ItemType) throw new FarseerException($"返回的类型：{returnType.Name}与缓存设置的item类型：{cacheKey.ItemType.Name} 不一致");

        // 缓存中，取出数据
        var item = cacheKey.GetItem(cacheId);
        if (item != null) // 缓存中存在，则直接返回
        {
            args.ReturnValue = item;
        }
        else // 缓存不存在，则执行业务，拿到数据后，并缓存
        {
            args.Proceed();
            if (args.ReturnValue != null) cacheKey.SaveItem(args.ReturnValue);
        }
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var returnType = ((MethodInfo)args.Method).ReturnType;
        var cacheKey   = CacheConfigure.Get(_key);
        var cacheId = args.Arguments.Count switch
        {
            1   => args.Arguments[0],
            > 1 => args.GetParamValueByAttribute<CacheIdAttribute>(),
            _   => null
        };

        if (cacheId           == null) throw new FarseerException($"未找到cacheId");
        if (cacheId.GetType() != cacheKey.DataKey.PropertyType) throw new FarseerException($"cacheId的类型：{cacheId.GetType().Name}与设置的类型：{cacheKey.DataKey.PropertyType.Name} 不一致");
        if (returnType        != cacheKey.ItemType) throw new FarseerException($"返回的类型：{returnType.Name}与缓存设置的item类型：{cacheKey.ItemType.Name} 不一致");

        // 缓存中，取出数据
        var item = cacheKey.GetItem(cacheId);
        if (item != null) // 缓存中存在，则直接返回
        {
            args.ReturnValue = item;
        }
        else // 缓存不存在，则执行业务，拿到数据后，并缓存
        {
            await args.ProceedAsync();
            if (args.ReturnValue != null) cacheKey.SaveItem(args.ReturnValue);
        }
    }
}