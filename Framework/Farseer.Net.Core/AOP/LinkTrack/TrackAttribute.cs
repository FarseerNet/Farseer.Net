using System;
using System.Linq;
using FS.Core.LinkTrack;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// 手动埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static)]
public class TrackAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var returnType = ((System.Reflection.MethodInfo)args.Method).ReturnType.Name;
        var argsString = string.Join(", ", args.Arguments.Select(p => $"{p.GetType().FullName} = {p}"));

        var callName = args.Method.DeclaringType != null ? $"{args.Method.DeclaringType.Name}.{args.Method.Name}" : $"{args.Method.Name}";
        using (FsLinkTrack.Track($"{returnType} {callName}({argsString})"))
        {
            args.Proceed();
        }
    }
}