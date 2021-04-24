using System;
using System.Threading;
using System.Threading.Tasks;
using Farseer.Net.Grpc;
using FS.DI;
using FS.Job.Configuration;
using FSS.GrpcService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using RpcResponse = FSS.GrpcService.RpcResponse;

namespace FS.Job
{
    /// <summary>
    /// 注册到服务端
    /// </summary>
    public class ServiceRegister
    {
        public IGrpcClient GrpcClient { get; set; }
        public IIocManager  IocManager { get; set; }

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
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var registerCenterClient = new RegisterCenter.RegisterCenterClient(GrpcChannel.ForAddress(_jobItemConfig.Server));
                        var rpc =registerCenterClient.Register(new RegisterRequest
                        {
                            ClientId = _clientId,
                            Endpoint = $"http://localhost:{_jobItemConfig.GrpcServicePort}"
                        });
                        IocManager.Logger<JobModule>().LogDebug($"注册到服务端,{_jobItemConfig.Server}");
                    }
                    catch (Exception e)
                    {
                        IocManager.Logger<ServiceRegister>().LogError(e,e.ToString());
                    }
                    Thread.Sleep(_jobItemConfig.ConnectFssServerTime);
                }
            });
        }
    }
}