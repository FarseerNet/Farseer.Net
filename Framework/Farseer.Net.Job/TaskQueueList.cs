using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.Job.Configuration;
using FS.Job.Entity;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    /// <summary>
    ///     从服务端摘取的任务列表
    /// </summary>
    public class TaskQueueList
    {
        /// <summary>
        ///     任务列表
        /// </summary>
        private static readonly Queue<TaskVO> _queue = new();

        /// <summary>
        ///     将任务添加到队列中
        /// </summary>
        public static void Enqueue(List<TaskVO> lstTask)
        {
            foreach (var task in lstTask.OrderBy(o => o.StartAt)) _queue.Enqueue(item: task);
        }

        /// <summary>
        /// 自动拉取任务
        /// </summary>
        public static void PullJob()
        {
            // 开启任务，自动拉取任务
            Task.Factory.StartNew(function: async () =>
            {
                // 如果是来自FSS.Service，则等待5秒后，再连接FSS服务
                var entryName = Assembly.GetEntryAssembly().EntryPoint.Module.Name;
                if (entryName == "FSS.Service.dll") await Task.Delay(5000);
                
                // 当前客户端支持的job
                var jobItemConfig = JobConfigRoot.Get();
                // 最大拉取数量（队列现存的数量）
                var maxPullCount = jobItemConfig.PullCount == 0 ? Environment.ProcessorCount : jobItemConfig.PullCount;
                while (true)
                {
                    // 本次要拉取的数量
                    var pullCount = maxPullCount - _queue.Count;

                    // 没有任务的时候，要主动拉取
                    if (pullCount > 0)
                    {
                        // 拉取任务
                        var lst = await TaskManager.PullAsync(pullCount);
                        // 从队列中，将已拉取的任务先取出来，目的是重新排序后入队
                        while (_queue.Count > 0)
                        {
                            lst.Add(_queue.Dequeue());
                        }
                        
                        // 将任务添加到队列中
                        Enqueue(lst);
                    }
                    Thread.Sleep(millisecondsTimeout: 500);
                }
            }, creationOptions: TaskCreationOptions.LongRunning);
        }

        /// <summary>
        ///     运行任务
        /// </summary>
        public static void RunJob()
        {
            // 开启任务，自动拉取任务
            Task.Factory.StartNew(function: async () =>
            {
                // 如果是来自FSS.Service，则等待5秒后，再连接FSS服务
                var entryName = Assembly.GetEntryAssembly().EntryPoint.Module.Name;
                if (entryName == "FSS.Service.dll") await Task.Delay(5000);
                
                while (true)
                {
                    // 没有任务的时候，休眠
                    if (_queue.Count == 0)
                    {
                        await Task.Delay(1000);
                        continue;
                    }

                    // 执行任务，这里不做等待,相当于开启一条线程执行
                    Task.Run(function: async () =>
                    {
                        await RunTask(task: _queue.Dequeue());
                    });

                    Thread.Sleep(millisecondsTimeout: 200);
                }
            }, creationOptions: TaskCreationOptions.LongRunning);
        }

        private static async Task RunTask(TaskVO task)
        {
            // 计划时间还没到
            var waitTimeSpan = task.StartAt - DateTime.Now;
            if (waitTimeSpan.TotalMilliseconds > 0) await Task.Delay(delay: waitTimeSpan);

            var message = $"任务组：TaskGroupId={task.TaskGroupId}，Caption={task.Caption}，JobName={task.JobName}";

            // JOB执行耗时计数器
            var sw = new Stopwatch();
            // 上下文
            var receiveContext = new ReceiveContext(task: task, sw: sw);
            var cts            = new CancellationTokenSource();

            // 开启激活任务，直到任务完成
            Task.Run(function: async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await receiveContext.ActivateTask();
                    await Task.Delay(millisecondsDelay: 10000, cancellationToken: cts.Token);
                }
            }, cancellationToken: cts.Token);

            // 业务是否有该调度任务的实现
            var jobInsName   = $"fss_job_{task.JobName}";
            var isRegistered = IocManager.Instance.IsRegistered(name: jobInsName);
            if (isRegistered is false)
            {
                IocManager.Instance.Logger<TaskQueueList>().LogWarning(message: $"未找到任务实现类：{message}");
                await receiveContext.FailAsync(log: new LogRequest
                {
                    LogLevel = LogLevel.Error,
                    Log      = $"未找到任务实现类：{message}",
                    CreateAt = DateTime.Now
                });
                return;
            }

            try
            {
                // 执行业务JOB
                var  fssJob = IocManager.GetService<IFssJob>(name: jobInsName);
                bool result;
                using (FsLinkTrack.TrackFss(clientHost: task.ClientHost, jobName: task.JobName, taskGroupId: task.TaskGroupId))
                {
                    // 执行具体任务（业务执行）
                    sw.Start();
                    result = await fssJob.Execute(context: receiveContext);
                }

                // 通知服务端，当前客户端执行结果
                if (result)
                    await receiveContext.SuccessAsync();
                else
                    await receiveContext.FailAsync();
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<TaskQueueList>().LogError(exception: e, message: $"taskGroupId={receiveContext.Meta.TaskGroupId},caption={receiveContext.Meta.Caption}：{e.Message}");
                await receiveContext.FailAsync(log: new LogRequest
                {
                    LogLevel = LogLevel.Error,
                    Log      = e.Message,
                    CreateAt = DateTime.Now
                });
            }
            finally
            {
                sw.Stop();
                cts.Cancel();
            }
        }
    }
}