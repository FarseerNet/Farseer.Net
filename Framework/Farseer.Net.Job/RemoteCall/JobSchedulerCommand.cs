using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FS.Job.Abstract;
using FS.Job.Entity;
using FS.Job.GrpcClient;
using FS.Utils.Common;
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
        public Task InvokeAsync(FssServer.FssServerClient client, IClientStreamWriter<ChannelRequest> requestStream, IAsyncStreamReader<CommandResponse> responseStream)
        {
            var task    = JsonConvert.DeserializeObject<JobSchedulerVO>(responseStream.Current.Data);
            var message = $"任务组：TaskGroupId={task.TaskGroupId}，Caption={task.Caption}，JobName={task.JobTypeName}，TaskId={task.TaskId}";

            ThreadPool.QueueUserWorkItem(async state =>
            {
                if (task.StartAt > DateTime.Now)
                {
                    var taskStartAt = task.StartAt - DateTime.Now;
                    IocManager.Logger<PrintCommand>().LogDebug($"客户端（{IpHelper.GetIps()[0].Address.MapToIPv4().ToString()}）收到{message}请求，等待{taskStartAt.TotalMilliseconds} ms后处理...");
                    Thread.Sleep(taskStartAt);
                }
                else
                {
                    IocManager.Logger<PrintCommand>().LogDebug($"客户端（{IpHelper.GetIps()[0].Address.MapToIPv4().ToString()}）收到{message}请求，立即处理。");
                }

                // 创建同步JOB状态的请求
                var jobInvokeClient = new JobInvokeClient(client, task.TaskGroupId, task.TaskId);
                var rpcJobInvoke    = jobInvokeClient.JobInvoke();

                // JOB执行耗时计数器
                var sw = new Stopwatch();
                // 上下文
                var receiveContext = new ReceiveContext(IocManager, rpcJobInvoke, task, sw);
                await receiveContext.UploadAsync();

                // 业务是否有该调度任务的实现
                var isRegistered = IocManager.IsRegistered($"fss_job_{task.JobTypeName}");
                if (isRegistered is false)
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
                    var         result = false;
                    LogResponse log    = null;
                    try
                    {
                        // 执行业务JOB
                        var fssJob = IocManager.Resolve<IFssJob>($"fss_job_{task.JobTypeName}");
                        sw.Start();
                        result = await fssJob.Execute(receiveContext);
                    }
                    catch (Exception e)
                    {
                        IocManager.Logger<JobSchedulerCommand>().LogError(e, e.ToString());
                        log = new LogResponse
                        {
                            LogLevel = (int) LogLevel.Error,
                            Log      = e.ToString(),
                            CreateAt = DateTime.Now.ToTimestamps()
                        };
                    }
                    finally
                    {
                        sw.Stop();
                    }

                    // 通知服务端，当前客户端执行结果
                    if (result) await receiveContext.SuccessAsync();
                    else await receiveContext.FailAsync(log);

                    // 告诉服务端，我已处理完
                    await rpcJobInvoke.RequestStream.CompleteAsync();

                    // 等待服务端返回结果
                    var rsp = await rpcJobInvoke.ResponseAsync;
                    IocManager.Resolve<IRemoteCommand>("fss_client_Print").Invoke(rsp.Data);
                }
                catch (Exception e)
                {
                    IocManager.Logger<JobSchedulerCommand>().LogError(e.Message);
                }
            });

            return Task.FromResult(0);
        }

        public void Invoke(string data) => throw new NotImplementedException();
    }
}