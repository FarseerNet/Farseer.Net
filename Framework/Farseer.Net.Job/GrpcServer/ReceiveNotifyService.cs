using System;
using System.Diagnostics;
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

        public override async Task JobInvoke(JobInvokeRequest request, IServerStreamWriter<JobInvokeResponse> responseStream, ServerCallContext context)
        {
            var message = $"（{request.JobTypeName}） 任务ID：{request.TaskId}、 任务：{request.Caption}、 执行时间：{request.NextAt.ToTimestamps():yyyy-MM-dd HH:mm:ss}";
            _ioc.Logger<ReceiveNotifyService>().LogInformation($"收到来自FSS平台的调度消息：{message}");

            // 上下文
            var sw             = new Stopwatch();
            var receiveContext = new ReceiveContext(_ioc, request, responseStream, sw);

            // 业务是否有该调度任务的实现
            var isRegistered = _ioc.IsRegistered($"fss_{request.JobTypeName}");
            if (!isRegistered)
            {
                _ioc.Logger<ReceiveNotifyService>().LogWarning($"未找到任务实现类：{message}");
                receiveContext.Fail(new LogResponse
                {
                    LogLevel = 4,
                    Log      = $"未找到任务实现类：{message}",
                    CreateAt = DateTime.Now.ToTimestamps()
                });
                return;
            }

            try
            {
                
                // 执行业务JOB
                var fssJob = _ioc.Resolve<IFssJob>($"fss_{request.JobTypeName}");
                
                sw.Start();
                // 执行JOB
                var execute = fssJob.Execute(receiveContext);
                sw.Stop();
                if (execute) receiveContext.Success();
                else receiveContext.Fail();
            }
            catch (Exception e)
            {
                _ioc.Logger<ReceiveNotifyService>().LogError(e, e.ToString());
                receiveContext.Fail(new LogResponse
                {
                    LogLevel = (int) LogLevel.Error,
                    Log      = e.ToString(),
                    CreateAt = DateTime.Now.ToTimestamps()
                });
            }

            await Task.FromResult(0);
        }
    }
}