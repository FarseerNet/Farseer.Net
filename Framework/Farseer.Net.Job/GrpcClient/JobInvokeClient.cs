using FSS.GrpcService;
using Grpc.Core;

namespace FS.Job.GrpcClient
{
    public class JobInvokeClient
    {
        private readonly int                       _taskId;
        private readonly string                    _serverHost;
        private readonly FssServer.FssServerClient _client;
        private readonly int                       _taskGroupId;

        public JobInvokeClient(FssServer.FssServerClient client, int taskGroupId, int taskId, string serverHost)
        {
            _taskId      = taskId;
            _serverHost  = serverHost;
            _client      = client;
            _taskGroupId = taskGroupId;
        }

        public AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> JobInvoke() => AsyncDuplexStreamingCall();

        private AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> AsyncDuplexStreamingCall()
        {
            var rpc = _client.JobInvoke(new Metadata
            {
                {"task_group_id", _taskGroupId.ToString()}, // 本次执行的任务ID
                {"task_id", _taskId.ToString()}, // 本次执行的任务ID
                {"server_host", _serverHost}     // 注册到FSS平台的标识
            });
            return rpc;
        }
    }
}