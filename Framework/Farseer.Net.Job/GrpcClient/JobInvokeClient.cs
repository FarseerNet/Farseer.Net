using FSS.GrpcService;
using Grpc.Core;

namespace FS.Job.GrpcClient
{
    public class JobInvokeClient
    {
        private readonly int                       _taskId;
        private readonly string                    _serverHost;
        private readonly FssServer.FssServerClient _client;

        public JobInvokeClient(FssServer.FssServerClient client, int taskId, string serverHost)
        {
            _taskId      = taskId;
            _serverHost  = serverHost;
            _client = client;
        }

        public AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> JobInvoke() => AsyncDuplexStreamingCall();

        private AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> AsyncDuplexStreamingCall()
        {
            var rpc = _client.JobInvoke(new Metadata
            {
                {"task_id", _taskId.ToString()}, // 本次执行的任务ID
                {"server_host", _serverHost}     // 注册到FSS平台的标识
            });
            
            //
            // // 开启线程持续读取服务端流
            // ThreadPool.QueueUserWorkItem(async state =>
            // {
            //     while (await rpc.ResponseAsync)
            //     {
            //         var iocName = $"fss_client_{rpc.ResponseStream.Current.Command}";
            //         if (!IocManager.IsRegistered(iocName))
            //             IocManager.Logger<ChannelClient>().LogWarning($"未知命令：{rpc.ResponseStream.Current.Command}");
            //         else
            //             // 交由具体功能处理实现执行
            //             await IocManager.Resolve<IRemoteCommand>(iocName).InvokeAsync(rpc.RequestStream, rpc.ResponseStream);
            //
            //         //if (rpc.ResponseStream.Current.Status) IocManager.Logger<JobModule>().LogDebug($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 收到FSS平台{_jobItemConfig.Server} 的心跳应答");
            //         //else IocManager.Logger<ChannelClient>().LogWarning($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 注册失败：{rpc.ResponseStream.Current.StatusMessage}");
            //     }
            // });
            return rpc;
        }
    }
}