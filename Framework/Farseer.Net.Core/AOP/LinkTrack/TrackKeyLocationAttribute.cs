using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// 关键位置埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackKeyLocationAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack)
        {
            args.Proceed();
            return;
        }

        var returnType = ((System.Reflection.MethodInfo)args.Method).ReturnType.Name;
        var argsString = string.Join(", ", args.Arguments.Select(p => $"{p.GetType().Name} = {p}"));

        var callName = args.Method.DeclaringType != null ? $"{args.Method.DeclaringType.Name}.{args.Method.Name}" : $"{args.Method.Name}";
        using (FsLinkTrack.TrackKeyLocation($"{returnType} {callName}({argsString})"))
        {
            args.Proceed();
        }
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack)
        {
            await args.ProceedAsync();
            return;
        }

        var returnType = ((MethodInfo)args.Method).ReturnType.GetGenericArguments()[0].Name;
        var argsString = string.Join(", ", args.Arguments.Select(p => $"{p.GetType().Name} = {p}"));

        var callName = args.Method.DeclaringType != null ? $"{args.Method.DeclaringType.Name}.{args.Method.Name}" : $"{args.Method.Name}";
        using (FsLinkTrack.TrackKeyLocation($"{returnType} {callName}({argsString})"))
        {
            await args.ProceedAsync();
        }
    }
}