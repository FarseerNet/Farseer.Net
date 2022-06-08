using System;
using System.Threading.Tasks;
using FS.Core.Abstract.Fss;
using FS.Core.LinkTrack;
using FS.Extends;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// FSS埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackFssAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { args.Proceed(); return; }
        
        var fssContext = args.GetParamValue<IFssContext>();
        using (FsLinkTrack.TrackFss(clientHost: fssContext.Meta.ClientHost, jobName: fssContext.Meta.JobName, taskGroupId: fssContext.Meta.TaskGroupId, fssContext.Meta.Data))
        {
            args.Proceed();
        }
    }
    
    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { await args.ProceedAsync(); return; }

        var fssContext = args.GetParamValue<IFssContext>();
        using (FsLinkTrack.TrackFss(clientHost: fssContext.Meta.ClientHost, jobName: fssContext.Meta.JobName, taskGroupId: fssContext.Meta.TaskGroupId, fssContext.Meta.Data))
        {
            await args.ProceedAsync();
        }
    }
}