using System;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.Extends;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using StackExchange.Redis;

namespace FS.Cache.Redis.Attribute;

/// <summary>
/// Redis埋点链路追踪
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method                                                                 | AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = true)]
[MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Instance | MulticastAttributes.Static, Inheritance = MulticastInheritance.Multicast)]
public class TrackRedisAttribute : MethodInterceptionAspect
{
    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var key   = args.GetParamValueWithoutException<RedisKey>();
        var value = args.GetParamValueWithoutException<RedisValue>();

        using (FsLinkTrack.TrackRedis(method: args.Method.Name, key: key, value))
        {
            args.Proceed();
        }
    }

    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        var key   = args.GetParamValueWithoutException<RedisKey>();
        var value = args.GetParamValueWithoutException<RedisValue>();

        using (FsLinkTrack.TrackRedis(method: args.Method.Name, key: key, value))
        {
            await args.ProceedAsync();
        }
    }
}