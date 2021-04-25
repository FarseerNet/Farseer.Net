using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.Windsor.Diagnostics.Extensions;
using Farseer.Net.Grpc;
using FS.DI;
using FS.Job.Configuration;
using FSS.GrpcService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using RpcResponse = FSS.GrpcService.RpcResponse;

namespace FS.Job.GrpcClient
{
    /// <summary>
    /// 注册到服务端
    /// </summary>
    public class ServiceRegister
    {
        public IGrpcClient GrpcClient { get; set; }
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        private readonly string _clientId;

        /// <summary>
        /// 配置
        /// </summary>
        private readonly JobItemConfig _jobItemConfig;

        public ServiceRegister(string clientId, JobItemConfig jobItemConfig)
        {
            _clientId      = clientId;
            _jobItemConfig = jobItemConfig;
        }

        /// <summary>
        /// 向服务端注册客户端连接
        /// </summary>
        public void Register()
        {
            IocManager.Logger<JobModule>().LogDebug($"尝试注册到FSS平台,{_jobItemConfig.Server}");
            var rpc = AsyncDuplexStreamingCall();

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await rpc.RequestStream.WriteAsync(new RegisterRequest
                        {
                            ClientId          = _clientId,
                            ReceiveNotifyPort = _jobItemConfig.GrpcServicePort
                        });
                    }
                    catch (Exception e)
                    {
                        var msg = e is RpcException rpcException ? rpcException.Status.Detail : e.Message;
                        IocManager.Logger<ServiceRegister>().LogError($"注册失败：{msg}");
                        rpc = AsyncDuplexStreamingCall();
                    }

                    Thread.Sleep(_jobItemConfig.ConnectFssServerTime);
                }
            });
        }

        private AsyncDuplexStreamingCall<RegisterRequest, RpcResponse> AsyncDuplexStreamingCall()
        {
            var grpcChannel          = GrpcChannel.ForAddress(_jobItemConfig.Server);
            var registerCenterClient = new RegisterCenter.RegisterCenterClient(grpcChannel);
            var rpc= registerCenterClient.Register();

            Task.Run(async () =>
            {
                while (await rpc.ResponseStream.MoveNext())
                {
                    if (rpc.ResponseStream.Current.Status) IocManager.Logger<JobModule>().LogDebug($"收到FSS平台的心跳应答,{_jobItemConfig.Server}");
                    else IocManager.Logger<ServiceRegister>().LogWarning($"注册失败：{rpc.ResponseStream.Current.StatusMessage}");
                }
            });
            return rpc;
        }
    }
}