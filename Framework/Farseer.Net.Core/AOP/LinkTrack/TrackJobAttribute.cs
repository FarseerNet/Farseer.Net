using System;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// Job埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackJobAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { args.Proceed(); return; }
        
        using (FsLinkTrack.TrackJob(args.Instance.GetType().Name))
        {
            args.Proceed();
        }
    }
    
    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { await args.ProceedAsync(); return; }

        using (FsLinkTrack.TrackJob(args.Instance.GetType().Name))
        {
            await args.ProceedAsync();
        }
    }
}