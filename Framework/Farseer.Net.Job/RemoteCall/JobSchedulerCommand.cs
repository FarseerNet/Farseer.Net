using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FS.Job.Abstract;
using FS.Job.Entity;
using FS.Job.GrpcClient;
using FSS.GrpcService;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FS.Job.RemoteCall
{
    /// <summary>
    /// 收到调度通知
    /// </summary>
    public class JobSchedulerCommand : IRemoteCommand
    {
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// 执行本地JOB
        /// </summary>
        public async Task InvokeAsync(FssServer.FssServerClient client, IClientStreamWriter<ChannelRequest> requestStream, IAsyncStreamReader<CommandResponse> responseStream)
        {
            var task    = JsonConvert.DeserializeObject<JobSchedulerVO>(responseStream.Current.Data);
            var message = $"（{task.JobTypeName}） 任务ID：{task.TaskId}、 任务：{task.Caption}、 执行时间：{task.NextAt.ToTimestamps():yyyy-MM-dd HH:mm:ss}";

            // 创建同步JOB状态的请求
            var jobInvokeClient = new JobInvokeClient(client, task.TaskGroupId, task.TaskId, task.ClientHost);
            var rpcJobInvoke    = jobInvokeClient.JobInvoke();
            // JOB执行耗时计数器
            var sw = new Stopwatch();
            // 上下文
            var receiveContext = new ReceiveContext(IocManager, rpcJobInvoke, task.NextAt.ToTimestamps(), sw);

            // 业务是否有该调度任务的实现
            var isRegistered = IocManager.IsRegistered($"fss_job_{task.JobTypeName}");
            if (!isRegistered)
            {
                IocManager.Logger<JobSchedulerCommand>().LogWarning($"未找到任务实现类：{message}");
                await receiveContext.FailAsync(new LogResponse
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
                var fssJob = IocManager.Resolve<IFssJob>($"fss_job_{task.JobTypeName}");

                sw.Start();
                // 执行JOB
                var execute = await fssJob.Execute(receiveContext);
                sw.Stop();
                if (execute) await receiveContext.SuccessAsync();
                else await receiveContext.FailAsync();
                await rpcJobInvoke.RequestStream.CompleteAsync();

                // 等待服务端返回结果
                var rsp = await rpcJobInvoke.ResponseAsync;
                IocManager.Logger<ChannelClient>().LogInformation(rsp.Data);
            }
            catch (Exception e)
            {
                IocManager.Logger<JobSchedulerCommand>().LogError(e, e.ToString());
                await receiveContext.FailAsync(new LogResponse
                {
                    LogLevel = (int) LogLevel.Error,
                    Log      = e.ToString(),
                    CreateAt = DateTime.Now.ToTimestamps()
                });
            }
        }
    }
}