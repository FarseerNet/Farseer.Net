using System;
using System.Net.Http;
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
        private AsyncDuplexStreamingCall<ChannelRequest, CommandResponse> _rpc;

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
        public AsyncDuplexStreamingCall<ChannelRequest, CommandResponse> Channel(string server, string job)
        {
            // 建立连接
            _grpcChannel = GrpcChannel.ForAddress(server, new GrpcChannelOptions
            {
                // 不检查服务端的http2有效性
                HttpHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                }
            });
            _registerCenterClient = new FssServer.FssServerClient(_grpcChannel);
            _rpc = _registerCenterClient.Channel(new Metadata
            {
                {"client_ip", IpHelper.GetIp} // 客户端IP
            });

            ThreadPool.QueueUserWorkItem(async state =>
            {
                try
                {
                    // 这里是阻塞的
                    await AsyncDuplexStreamingCall(server, job);
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

                        IocManager.Logger<ChannelClient>().LogWarning($"{server}：{msg}");
                    }
                    else
                        IocManager.Logger<ChannelClient>().LogError($"{server}：{msg}");

                    Thread.Sleep(1000 * 10);
                }
                finally
                {
                    Close();
                }
            }, TaskCreationOptions.LongRunning);
            return _rpc;
        }

        private async Task AsyncDuplexStreamingCall(string server, string job)
        {
            IocManager.Logger<JobModule>().LogInformation($"正在连接FSS平台{server}，注册 {job} 任务");

            // 请求注册
            await _rpc.RequestStream.WriteAsync(new ChannelRequest
            {
                Command   = "Register",
                RequestAt = DateTime.Now.ToTimestamps(),
                Data      = job
            });

            try
            {

                // 持续读取服务端流
                while (await _rpc.ResponseStream.MoveNext())
                {
                    var iocName = $"fss_client_{_rpc.ResponseStream.Current.Command}";
                    if (!IocManager.IsRegistered(iocName))
                        IocManager.Logger<ChannelClient>().LogWarning($"未知命令：{_rpc.ResponseStream.Current.Command}");
                    else
                        // 交由具体功能处理实现执行
                        IocManager.Resolve<IRemoteCommand>(iocName).InvokeAsync(_registerCenterClient, _rpc.RequestStream, _rpc.ResponseStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void Close()
        {
            if (_rpc != null) _rpc.Dispose();
            if (_grpcChannel != null) _grpcChannel.Dispose();
        }
    }
}