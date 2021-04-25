using System;
using System.Threading.Tasks;
using FS.DI;
using FSS.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using FS.Extends;

namespace FS.Job.GrpcServer
{
    public class ReceiveNotifyService : FSS.Client.ReceiveNotify.ReceiveNotifyBase
    {
        private readonly IIocManager _ioc;

        public ReceiveNotifyService(IIocManager ioc)
        {
            _ioc = ioc;
        }

        public override Task<JobInvokeResponse> JobInvoke(JobInvokeRequest request, ServerCallContext context)
        {
            _ioc.Logger<ReceiveNotifyService>().LogInformation($"收到来自FSS的调度：（{request.JobTypeName}）{request.TaskId}、{request.Caption}、{request.StartAt.ToTimestamps():yyyy-MM-dd HH:mm:ss}");
            
            // 业务是否有该调度任务的实现
            var isRegistered = _ioc.IsRegistered(request.JobTypeName);
            if (!isRegistered) return Task.FromResult(new JobInvokeResponse {TaskId = request.TaskId, NextAt = request.StartAt + (1000 * 60), Progress = 0, Status = 2});

            try
            {
                // 执行业务JOB
                var fssJob = _ioc.Resolve<IFssJob>(request.JobTypeName);
                return Task.FromResult(new JobInvokeResponse
                {
                    TaskId   = request.TaskId,
                    NextAt   = request.StartAt + (1000 * 60),
                    Progress = 0,
                    Status   = fssJob.Invoke() ? 3 : 2
                });
            }
            catch (Exception e)
            {
                _ioc.Logger<ReceiveNotifyService>().LogError(e, e.ToString());
                return Task.FromResult(new JobInvokeResponse
                {
                    TaskId   = request.TaskId,
                    NextAt   = request.StartAt + (1000 * 60),
                    Progress = 0,
                    Status   = 2
                });
            }
        }
    }
}