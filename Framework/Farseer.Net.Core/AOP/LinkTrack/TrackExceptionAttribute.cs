using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FS.Core.LinkTrack;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// 异常跟踪：当使用链路追踪时，该异常会被记录并统计
/// </summary>
[PSerializable]
public sealed class TrackExceptionAttribute : OnExceptionAspect
{
    /// <summary>
    /// 异常跟踪：当使用链路追踪时，该异常会被记录并统计
    /// </summary>
    public override void OnException(MethodExecutionArgs args)
    {
        var parameterInfos = args.Method.GetParameters();

        // 方法签名 (int MethodName)
        var method = AppendTypeName(args);
        AppendParamName(method, parameterInfos);

        // 入参值
        var methodParams = new Dictionary<string, string>();
        for (var index = 0; index < parameterInfos.Length; index++)
        {
            methodParams[parameterInfos[index].Name] = args.Arguments[index].ToString();
        }

        // 异常类型
        var exceptionTypeName = args.Exception.GetType().Name;

        // 方法调用模拟
        var methodCall = AppendTypeName(args);
        AppendArguments(methodCall, args.Arguments);

        FsLinkTrack.Exception(method.ToString(), methodParams, exceptionTypeName, args.Exception.Message, methodCall.ToString());
    }

    /// <summary>
    /// 拼接参数名称
    /// </summary>
    private void AppendParamName(StringBuilder method, ParameterInfo[] parameterInfos)
    {

        method.Append($"(");
        // 入参签名 (int x, int y)
        method.Append(parameterInfos.Aggregate(string.Empty, (current, parameter) => current + $"{parameter.ParameterType.Name} {parameter.Name}, ").Trim(',', ' '));
        method.Append($")");
    }

    /// <summary>
    /// 拼接方法返回值、方法名称
    /// </summary>
    private StringBuilder AppendTypeName(MethodExecutionArgs args)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(args.Method.DeclaringType.FullName);
        if (args.Method.DeclaringType.IsGenericType)
        {
            var genericArguments = args.Method.DeclaringType.GetGenericArguments();
            AppendGenericArguments(stringBuilder, genericArguments);
        }
        stringBuilder.Append('.');
        stringBuilder.Append(args.Method.Name);
        return stringBuilder;
    }

    private void AppendGenericArguments(StringBuilder stringBuilder, Type[] genericArguments)
    {
        stringBuilder.Append('<');
        for (var i = 0; i < genericArguments.Length; i++)
        {
            if (i > 0)
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(genericArguments[i].Name);
        }
        stringBuilder.Append('>');
    }

    private void AppendArguments(StringBuilder stringBuilder, Arguments arguments)
    {
        stringBuilder.Append('(');
        for (var i = 0; i < arguments.Count; i++)
        {
            if (i > 0)
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(arguments[i]);
        }
        stringBuilder.Append(')');
    }
}