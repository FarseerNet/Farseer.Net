using System.Threading.Tasks;
using Farseer.Net.Job;
using Grpc.Core;

namespace FS.Job.GrpcServer
{
    public class CallJobService : Farseer.Net.Job.CallJob.CallJobBase
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<RpcResponse> Run(DefaultRequest request, ServerCallContext context)
        {
            return base.Run(request, context);
        }
    }
}