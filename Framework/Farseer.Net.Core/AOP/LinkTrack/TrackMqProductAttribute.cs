using System;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ;
using FS.Core.LinkTrack;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// MQ生产埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackMqProductAttribute : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private MqType _mqType;

    /// <summary>
    /// MQ生产埋点链路追踪
    /// </summary>
    /// <param name="mqType">MQ类型</param>
    public TrackMqProductAttribute(MqType mqType)
    {
        _mqType = mqType;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { args.Proceed(); return; }
        
        if (args.Instance is not IMqProduct mqProduct) throw new System.Exception($"发送消息的方法所在类，必须继承{nameof(IMqProduct)}");

        using (FsLinkTrack.TrackMqProduct(method: $"{_mqType.ToString()}.Send.{mqProduct.QueueName}"))
        {
            args.Proceed();
        }
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { await args.ProceedAsync(); return; }

        if (args.Instance is not IMqProduct mqProduct) throw new System.Exception($"发送消息的方法所在类，必须继承{nameof(IMqProduct)}");

        using (FsLinkTrack.TrackMqProduct(method: $"{_mqType.ToString()}.Send.{mqProduct.QueueName}"))
        {
            await args.ProceedAsync();
        }
    }
}