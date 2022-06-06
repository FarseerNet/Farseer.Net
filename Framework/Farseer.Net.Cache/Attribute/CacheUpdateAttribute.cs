using System;
using System.Reflection;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Cache.Attribute;

/// <summary>
/// 缓存更新or添加
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class CacheUpdateAttribute : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private string _key;
    public CacheUpdateAttribute(string key)
    {
        _key = key;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var cacheKey   = CacheConfigure.Get(_key);
        var returnType = ((MethodInfo)args.Method).ReturnType;
        if (returnType != cacheKey.ItemType) throw new Exception($"更新缓存时，方法返回值必须是{cacheKey.ItemType.Name}");
        args.Proceed();
        
        cacheKey.Save(args.ReturnValue);
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var cacheKey   = CacheConfigure.Get(_key);
        var returnType = ((MethodInfo)args.Method).ReturnType;
        if (returnType != cacheKey.ItemType) throw new Exception($"更新缓存时，方法返回值必须是{cacheKey.ItemType.Name}");
        
        await args.ProceedAsync();
        
        cacheKey.Save(args.ReturnValue);
    }
}