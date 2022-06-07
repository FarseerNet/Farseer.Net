using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FS.DI;
using Microsoft.Extensions.Logging;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.AOP;

/// <summary>
/// 日志打印
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class LogAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        // 方法调用模拟
        var methodCall = AppendTypeName(args.Method);
        AppendArguments(methodCall, args.Arguments);
        var sw = Stopwatch.StartNew();
        args.Proceed();
        methodCall.Append($"; 耗时：{sw.ElapsedMilliseconds:N0} ms");
        IocManager.Instance.Logger<LogAttribute>().LogInformation(methodCall.ToString());
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        // 方法调用模拟
        var methodCall = AppendTypeName(args.Method);
        AppendArguments(methodCall, args.Arguments);
        var sw = Stopwatch.StartNew();
        await args.ProceedAsync();
        methodCall.Append($"; 耗时：{sw.ElapsedMilliseconds:N0} ms");
        IocManager.Instance.Logger<LogAttribute>().LogInformation(methodCall.ToString());
    }

    /// <summary>
    /// 拼接方法返回值、方法名称
    /// </summary>
    private StringBuilder AppendTypeName(MethodBase method)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(method.DeclaringType.FullName);
        if (method.DeclaringType.IsGenericType)
        {
            var genericArguments = method.DeclaringType.GetGenericArguments();
            AppendGenericArguments(stringBuilder, genericArguments);
        }
        stringBuilder.Append('.');
        stringBuilder.Append(method.Name);
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