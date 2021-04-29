using System;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FS.Job.Abstract;
using FS.Job.Configuration;
using FS.Utils.Common;
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
        public  IIocManager                                               IocManager { get; set; }
        private GrpcChannel                                               _grpcChannel;
        private FssServer.FssServerClient                                 _registerCenterClient;
        private AsyncDuplexStreamingCall<ChannelRequest, CommandResponse> _rpc; //, DateTime.UtcNow.AddSeconds(5)

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
        public void Channel(string server, string[] jobs)
        {
            var arrJob = string.Join(",", jobs);
            ThreadPool.QueueUserWorkItem(async state =>
            {
                while (true)
                {
                    try
                    {
                        IocManager.Logger<JobModule>().LogInformation($"正在连接FSS平台{server}，注册 {arrJob} 任务");
                        // 这里是阻塞的
                        await AsyncDuplexStreamingCall(server, arrJob);
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException != null) e = e.InnerException;
                        var msg                         = e.Message;
                        if (e is RpcException rpcException)
                        {
                            if (!string.IsNullOrWhiteSpace(rpcException.Status.Detail))
                            {
                                msg = rpcException.Status.Detail;
                            }
                            IocManager.Logger<ChannelClient>().LogWarning($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {server}，注册失败：{msg}");
                        }
                        else
                            IocManager.Logger<ChannelClient>().LogError($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {server}，注册失败：{msg}");

                        Thread.Sleep(1000 * 10);
                    }
                    finally
                    {
                        Close();
                    }

                    Thread.Sleep(3000);
                }
            });
        }

        private async Task AsyncDuplexStreamingCall(string server, string arrJob)
        {
            _grpcChannel          = GrpcChannel.ForAddress(server);
            _registerCenterClient = new FssServer.FssServerClient(_grpcChannel);
            _rpc = _registerCenterClient.Channel(new Metadata
            {
                {"client_ip", IpHelper.GetIps()[0].Address.MapToIPv4().ToString()} // 客户端IP
            });

            // 请求注册
            await _rpc.RequestStream.WriteAsync(new ChannelRequest
            {
                Command   = "Register",
                RequestAt = DateTime.Now.ToTimestamps(),
                Data      = arrJob
            });

            // 向服务器发送心跳,2S一次
            ThreadPool.QueueUserWorkItem(async state =>
            {
                while (true)
                {
                    try
                    {
                        // 请求注册
                        await _rpc.RequestStream.WriteAsync(new ChannelRequest
                        {
                            Command   = "Heartbeat",
                            RequestAt = DateTime.Now.ToTimestamps()
                        });
                        await Task.Delay(5000);
                    }
                    catch
                    {
                        return;
                    }
                }
            });

            // 持续读取服务端流
            while (await _rpc.ResponseStream.MoveNext())
            {
                var iocName = $"fss_client_{_rpc.ResponseStream.Current.Command}";
                if (!IocManager.IsRegistered(iocName))
                    IocManager.Logger<ChannelClient>().LogWarning($"未知命令：{_rpc.ResponseStream.Current.Command}");
                else
                    // 交由具体功能处理实现执行
                    await IocManager.Resolve<IRemoteCommand>(iocName).InvokeAsync(_registerCenterClient, _rpc.RequestStream, _rpc.ResponseStream);
            }
        }

        private void Close()
        {
            if (_rpc != null) _rpc.Dispose();
            if (_grpcChannel != null) _grpcChannel.Dispose();
        }
    }
}