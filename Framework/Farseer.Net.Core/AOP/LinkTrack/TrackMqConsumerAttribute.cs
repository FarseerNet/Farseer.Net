using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ;
using FS.Core.LinkTrack;
using FS.DI;
using FS.Extends;
using Newtonsoft.Json;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace FS.Core.AOP.LinkTrack;

/// <summary>
/// MQ消费埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackMqConsumerAttribute : MethodInterceptionAspect
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private MqType _mqType;

    /// <summary>
    /// MQ消费埋点链路追踪
    /// </summary>
    /// <param name="mqType">MQ类型</param>
    public TrackMqConsumerAttribute(MqType mqType)
    {
        _mqType = mqType;
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        if (!FsLinkTrack.IsUseLinkTrack) { await args.ProceedAsync(); return; }

        switch (_mqType)
        {
            case MqType.Queue:
                await TrackQueue(args: args);
                return;
            case MqType.Rabbit:
                await TrackRabbit(args);
                return;
            case MqType.RedisStream:
                await TrackRedisStream(args);
                return;
            default:
                await args.ProceedAsync();
                break;
        }
    }

    private async Task TrackQueue(MethodInterceptionArgs args)
    {
        var useLinkTrack = args.Instance.GetType().Module.Name != "Farseer.Net.LinkTrack.dll";
        if (useLinkTrack)
        {
            // 由于全链路追踪的数据是通过QueueConsumer消费后写入ES的，所以LinkTrackConsumer的消费不能再次调用消费追踪，否则会重复
            var customAttribute = args.Instance.GetType().GetCustomAttribute<Abstract.MQ.Queue.ConsumerAttribute>();
            var lst             = args.GetParamValue<IEnumerable<object>>();
            using (FsLinkTrack.TrackMqConsumer(endPort: "127.0.0.1", customAttribute.Name, method: "QueueConsumer", $"本次消费{lst.Count()}条"))
            {
                await args.ProceedAsync();
            }
            return;
        }
        await args.ProceedAsync();
    }

    private async Task TrackRabbit(MethodInterceptionArgs args)
    {
        var    consumerAttribute = args.Instance.GetType().GetCustomAttribute<Abstract.MQ.Rabbit.ConsumerAttribute>();
        string message;
        if (args.Arguments[0] is IEnumerable<string> lst)
        {
            message = $"本次消费{lst.Count()}条";
        }
        else
        {
            message = args.Arguments[0].ToString();
        }
        using (FsLinkTrack.TrackMqConsumer(endPort: consumerAttribute.Server, queueName: consumerAttribute.QueueName, method: "RabbitConsumer", message))
        {
            await args.ProceedAsync();
        }
    }

    private async Task TrackRedisStream(MethodInterceptionArgs args)
    {
        var consumerAttribute = args.Instance.GetType().GetCustomAttribute<Abstract.MQ.RedisStream.ConsumerAttribute>();
        var consumeContext    = args.GetParamValue<Abstract.MQ.RedisStream.ConsumeContext>();
        var messages          = consumeContext.RedisStreamMessages.Select(o => o.Message);
        using (FsLinkTrack.TrackMqConsumer(endPort: consumerAttribute.Server, queueName: $"{consumerAttribute.QueueName}/{consumerAttribute.GroupName}", method: "RedisStreamConsumer", JsonConvert.SerializeObject(messages)))
        {
            await args.ProceedAsync();
        }
    }
}