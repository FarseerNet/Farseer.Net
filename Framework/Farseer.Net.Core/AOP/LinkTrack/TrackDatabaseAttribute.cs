using System;
using System.Threading.Tasks;
using FS.Core.Abstract.Data;
using FS.Core.Abstract.MQ;
using FS.Core.LinkTrack;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// 数据库埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackDatabaseAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { args.Proceed(); return; }
        
        if (args.Instance is not IDbExecutor dbExecutor) throw new System.Exception($"发送消息的方法所在类，必须继承{nameof(IMqProduct)}");

        using (FsLinkTrack.TrackDatabase(method: $"{args.Method.Name} IsTransaction={dbExecutor.IsTransaction}", dbExecutor.ConnectionString))
        {
            args.Proceed();
        }
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { await args.ProceedAsync(); return; }

        if (args.Instance is not IDbExecutor dbExecutor) throw new System.Exception($"发送消息的方法所在类，必须继承{nameof(IMqProduct)}");

        using (FsLinkTrack.TrackDatabase(method: $"{args.Method.Name} IsTransaction={dbExecutor.IsTransaction}", dbExecutor.ConnectionString))
        {
            await args.ProceedAsync();
        }
    }
}