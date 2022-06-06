using System;
using System.Reflection;
using FS.DI;
using PostSharp.Aspects;

namespace FS.Extends;

public static class PostSharpExtend
{
    /// <summary>
    /// 根据参数名称，找到参数值所在的索引位置
    /// </summary>
    public static int GetParamIndex(this MethodInterceptionArgs args, string paramName)
    {
        var parameterInfos = args.Method.GetParameters();
        for (int i = 0; i < parameterInfos.Length; i++)
        {
            if (parameterInfos[i].Name == paramName) return i;
        }

        throw new FarseerException($"未找到参数名称：{paramName}");
    }

    /// <summary>
    /// 根据参数名称，找到参数值所在的索引位置
    /// </summary>
    public static object GetParamValue(this MethodInterceptionArgs args, string paramName)
    {
        var parameterInfos = args.Method.GetParameters();
        for (var i = 0; i < parameterInfos.Length; i++)
        {
            if (parameterInfos[i].Name == paramName) return args.Arguments[i];
        }

        throw new FarseerException($"未找到参数名称：{paramName}");
    }

    /// <summary>
    /// 根据参数名称，找到参数值
    /// </summary>
    public static TValue GetParamValue<TValue>(this MethodInterceptionArgs args, string paramName)
    {
        var parameterInfos = args.Method.GetParameters();
        for (var i = 0; i < parameterInfos.Length; i++)
        {
            if (parameterInfos[i].Name == paramName) return (TValue)args.Arguments[i];
        }

        throw new FarseerException($"未找到参数名称：{paramName}");
    }

    /// <summary>
    /// 根据参数类型，找到参数值
    /// </summary>
    public static TValue GetParamValue<TValue>(this MethodInterceptionArgs args)
    {
        foreach (var argVal in args.Arguments)
        {
            if (argVal is TValue val) return val;
        }

        throw new FarseerException($"未找到参数类型：{typeof(TValue).Name}");
    }

    /// <summary>
    /// 根据参数类型，找到参数值
    /// </summary>
    public static TValue GetParamValueWithoutException<TValue>(this MethodInterceptionArgs args)
    {
        foreach (var argVal in args.Arguments)
        {
            if (argVal is TValue val) return val;
        }
        return default;
    }

    /// <summary>
    /// 根据标记特性的入参，获取值
    /// </summary>
    public static object GetParamValueByAttribute<TAttribute>(this MethodInterceptionArgs args) where TAttribute : Attribute
    {
        var parameterInfos = args.Method.GetParameters();
        for (var index = 0; index < parameterInfos.Length; index++)
        {
            var cacheIdAttribute = parameterInfos[index].GetCustomAttribute<TAttribute>();
            if (cacheIdAttribute != null)
            {
                return args.Arguments[index];
            }
        }
        throw new FarseerException($"未找到标记：{typeof(TAttribute).Name} 的入参");
    }
}