using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.Abstract.Cache;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Cache.Attribute;

/// <summary>
/// 缓存读取
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class CacheAttribute : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private string _key;
    public CacheAttribute(string key)
    {
        _key = key;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var returnType = ((MethodInfo)args.Method).ReturnType;
        if (returnType.Namespace != "System.Collections.Generic" && returnType.Name != "IList`1")
        {
            var returnInterfaces = returnType.GetInterfaces();
            if (!returnInterfaces.Contains(typeof(IList))) throw new Exception($"要缓存的数据，必须是继承IList接口");
        }
        var cacheKey = CacheConfigure.Get(_key);

        // 缓存中，取出数据
        var lst = cacheKey.Get();
        if (lst != null) // 缓存中存在，则直接返回
        {
            SetReturnValue(args, lst, returnType, cacheKey);
        }
        else // 缓存不存在，则执行业务，拿到数据后，并缓存
        {
            args.Proceed();
            cacheKey.Set((IList)args.ReturnValue);
        }
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var cacheKey         = CacheConfigure.Get(_key);
        var returnType       = ((MethodInfo)args.Method).ReturnType;
        if (returnType.Namespace != "System.Collections.Generic" && returnType.Name != "IList`1")
        {
            var returnInterfaces = returnType.GetInterfaces();
            if (!returnInterfaces.Contains(typeof(IList))) throw new Exception($"要缓存的数据，必须是继承IList接口");
        }

        // 缓存中，取出数据
        var lst = cacheKey.Get();
        if (lst != null) // 缓存中存在，则直接返回
        {
            SetReturnValue(args, lst, returnType, cacheKey);
        }
        else // 缓存不存在，则执行业务，拿到数据后，并缓存
        {
            await args.ProceedAsync();
            cacheKey.Set((IList)args.ReturnValue);
        }
    }

    /// <summary>
    /// 当拿到缓存数据时，返回给实现被拦截的方法
    /// </summary>
    private void SetReturnValue(MethodInterceptionArgs args, IList lst, Type returnType, CacheKey2 cacheKey)
    {
        var cacheListType = lst.GetType();

        // 写入的类型与返回的类型一致时，直接返回
        if (returnType == cacheListType) args.ReturnValue = lst;
        else
        {
            if (returnType.IsArray) // 数组
            {
                var arrayList = new ArrayList();
                foreach (var val in lst)
                {
                    arrayList.Add(val);
                }
                args.ReturnValue = arrayList.ToArray(cacheKey.ItemType);
            }
            else
            {
                var count = lst.Count;

                // 根据返回类型，创建集合实例
                var returnInstance = cacheKey.CreateNewList(returnType, count);
                foreach (var val in lst)
                {
                    returnInstance.Add(val);
                }
                args.ReturnValue = returnInstance;
            }
        }
    }
}