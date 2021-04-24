using System.Threading.Tasks;
using FSS.Client;
using Grpc.Core;
using RpcResponse = FSS.Client.RpcResponse;

namespace FS.Job.GrpcServer
{
    public class ReceiveNotifyService : FSS.Client.ReceiveNotify.ReceiveNotifyBase
    {
        public override Task<RpcResponse> JobInvoke(JobInvokeRequest request, ServerCallContext context)
        {
            return base.JobInvoke(request, context);
        }
    }
}