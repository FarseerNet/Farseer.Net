using System;
using System.Reflection;
using System.Threading.Tasks;
using FS.Extends;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Cache.Attribute;

/// <summary>
/// 缓存删除
/// 当方法没有入参时，则认为清空当前缓存。
/// 当方法只有1个入参时，则认为这个入参是唯一标识的值。
/// 当方法有2个以上入参，则需使用<see cref="FS.Cache.Attribute.CacheIdAttribute"/>标记缓存唯一标识。
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class CacheRemoveAttribute : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private string _key;
    public CacheRemoveAttribute(string key)
    {
        _key = key;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var cacheKey = CacheConfigure.Get(_key);
        var cacheId = args.Arguments.Count switch
        {
            1   => args.Arguments[0],
            > 1 => args.GetParamValueByAttribute<CacheIdAttribute>(),
            _   => null
        };

        args.Proceed();

        if (cacheId != null) cacheKey.Remove(cacheId);
        else cacheKey.Clear();
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var cacheKey = CacheConfigure.Get(_key);
        var cacheId = args.Arguments.Count switch
        {
            1   => args.Arguments[0],
            > 1 => args.GetParamValueByAttribute<CacheIdAttribute>(),
            _   => null
        };

        await args.ProceedAsync();

        if (cacheId != null) cacheKey.Remove(cacheId);
        else cacheKey.Clear();
    }
}