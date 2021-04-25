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

        public override Task JobInvoke(JobInvokeRequest request, IServerStreamWriter<JobInvokeResponse> responseStream, ServerCallContext context)
        {
            var message = $"（{request.JobTypeName}） 任务ID：{request.TaskId}、 任务：{request.Caption}、 执行时间：{request.StartAt.ToTimestamps():yyyy-MM-dd HH:mm:ss}";
            _ioc.Logger<ReceiveNotifyService>().LogInformation($"收到来自FSS平台的调度消息：{message}");

            // 业务是否有该调度任务的实现
            var isRegistered = _ioc.IsRegistered(request.JobTypeName);
            if (!isRegistered)
            {
                _ioc.Logger<ReceiveNotifyService>().LogWarning($"未找到任务实现类：{message}");
                return Task.FromResult(new JobInvokeResponse {TaskId = request.TaskId, NextAt = request.StartAt + (1000 * 60), Progress = 0, Status = 2});
            }

            // 上下文
            var receiveContext = new ReceiveContext(_ioc, request, responseStream);

            try
            {
                // 执行业务JOB
                var fssJob = _ioc.Resolve<IFssJob>(request.JobTypeName);

                // 执行JOB
                var execute                          = fssJob.Execute(receiveContext);
                if (execute) receiveContext.Progress = 100;

                // 返回结果
                return Task.FromResult(new JobInvokeResponse
                {
                    TaskId   = request.TaskId,
                    NextAt   = receiveContext.NextAt,
                    Progress = receiveContext.Progress,
                    Status   = execute ? 3 : 2
                });
            }
            catch (Exception e)
            {
                _ioc.Logger<ReceiveNotifyService>().LogError(e, e.ToString());
                return Task.FromResult(new JobInvokeResponse
                {
                    TaskId   = request.TaskId,
                    NextAt   = 0,
                    Progress = 0,
                    Status   = 2
                });
            }
        }
    }
}