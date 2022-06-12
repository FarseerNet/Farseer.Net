using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.Abstract.Cache;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Cache.Attribute;

/// <summary>
/// 读取缓存数量
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class CacheCountAttribute : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private string _key;
    public CacheCountAttribute(string key)
    {
        _key = key;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var cacheKey = CacheConfigure.Get(_key);
        // 缓存中存在数据，则直接返回
        var count    = cacheKey.Count();
        if (count > 0 || cacheKey.Exists())
        {
            args.ReturnValue = count;
            return;
        }
        args.Proceed();
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var cacheKey = CacheConfigure.Get(_key);
        // 缓存中存在数据，则直接返回
        var count = cacheKey.Count();
        if (count > 0 || cacheKey.Exists())
        {
            args.ReturnValue = count;
            return;
        }
        await args.ProceedAsync();
    }
}