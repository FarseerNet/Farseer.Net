using System;
using System.Threading.Tasks;
using FSS.Client;
using Grpc.Core;

namespace FS.Job.GrpcServer
{
    public class ReceiveNotifyService : FSS.Client.ReceiveNotify.ReceiveNotifyBase
    {
        public override Task<JobInvokeResponse> JobInvoke(JobInvokeRequest request, ServerCallContext context)
        {
            Console.WriteLine(request.Caption);
            request.TaskId = 0;
            return Task.FromResult(new JobInvokeResponse
            {
            });
        }
    }
}