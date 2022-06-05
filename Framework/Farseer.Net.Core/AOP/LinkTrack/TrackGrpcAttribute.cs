using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Core.Http;
using FS.Core.LinkTrack;
using FS.Extends;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// Grpc埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackGrpcAttribute : MethodInterceptionAspect
{
    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        // 找到context参数
        var     clientInterceptorContext = GetClientInterceptorContext(args);
        dynamic context                  = args.Arguments[clientInterceptorContext];

        using (FsLinkTrack.TrackGrpc(server: context.Method.ServiceName, action: context.Method.Name))
        {
            await args.ProceedAsync();
        }
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        // 找到context参数
        var     clientInterceptorContext = GetClientInterceptorContext(args);
        dynamic context                  = args.Arguments[clientInterceptorContext];

        using (FsLinkTrack.TrackGrpc(server: context.Method.ServiceName, action: context.Method.Name))
        {
            args.Proceed();
        }
    }

    /// <summary>
    /// 找到ClientInterceptorContext参数
    /// </summary>
    private int GetClientInterceptorContext(MethodInterceptionArgs args)
    {
        var parameterInfos = args.Method.GetParameters();
        for (var index = 0; index < parameterInfos.Length; index++)
        {
            var parameterInfo = parameterInfos[index];
            // 找到context
            if (parameterInfo.ParameterType.Name == "ClientInterceptorContext`2" && parameterInfo.ParameterType.Namespace == "Grpc.Core.Interceptors")
            {
                return index;
            }
        }
        throw new System.Exception($"未找到Grpc.Core.Interceptors.ClientInterceptorContext`2类型的入参");
    }
}