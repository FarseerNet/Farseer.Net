using System;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FS.Job.Abstract;
using FS.Job.Configuration;
using FSS.GrpcService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace FS.Job.GrpcClient
{
    /// <summary>
    /// 注册到服务端
    /// </summary>
    public class ChannelClient
    {
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        private readonly JobItemConfig _jobItemConfig;

        public ChannelClient(JobItemConfig jobItemConfig)
        {
            _jobItemConfig = jobItemConfig;
        }

        /// <summary>
        /// 向服务端注册客户端连接
        /// </summary>
        public void Channel(string[] jobs)
        {
            var arrJob = string.Join(",", jobs);
            foreach (var server in _jobItemConfig.Server.Split(','))
            {
                ThreadPool.QueueUserWorkItem(async state =>
                {
                    while (true)
                    {
                        try
                        {
                            IocManager.Logger<JobModule>().LogInformation($"正在连接FSS平台{server}，注册 {arrJob} 任务");
                            await AsyncDuplexStreamingCall(server, arrJob);
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException != null) e = e.InnerException;
                            var msg                         = e.Message;
                            if (e is RpcException rpcException)
                            {
                                if (!string.IsNullOrWhiteSpace(rpcException.Status.Detail)) msg = rpcException.Status.Detail;
                            }
                            
                            IocManager.Logger<ChannelClient>().LogError($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 注册失败：{msg}");
                        }

                        Thread.Sleep(3000);
                    }
                });
            }
        }

        private async Task<AsyncDuplexStreamingCall<ChannelRequest, CommandResponse>> AsyncDuplexStreamingCall(string server, string arrJob)
        {
            var grpcChannel          = GrpcChannel.ForAddress(server);
            var registerCenterClient = new FssServer.FssServerClient(grpcChannel);
            var rpc = registerCenterClient.Channel(new Metadata
            {
                {"client_ip", "127.0.0.1"} // 客户端IP
            }, DateTime.UtcNow.AddSeconds(5));

            
            // 请求注册
            await rpc.RequestStream.WriteAsync(new ChannelRequest
            {
                Command   = "Register",
                RequestAt = DateTime.Now.ToTimestamps(),
                Data      = arrJob
            });

            // 持续读取服务端流
            while (await rpc.ResponseStream.MoveNext())
            {
                var iocName = $"fss_client_{rpc.ResponseStream.Current.Command}";
                if (!IocManager.IsRegistered(iocName))
                    IocManager.Logger<ChannelClient>().LogWarning($"未知命令：{rpc.ResponseStream.Current.Command}");
                else
                    // 交由具体功能处理实现执行
                    await IocManager.Resolve<IRemoteCommand>(iocName).InvokeAsync(registerCenterClient, rpc.RequestStream, rpc.ResponseStream);
            }

            return rpc;
        }
    }
}