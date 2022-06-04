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
    /// 根据参数名称，找到参数值所在的索引位置
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
}