using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.Job.Entity;
using Microsoft.Extensions.Logging;

namespace FS.Job.TaskQueue
{
    /// <summary>
    /// 从服务端摘取的任务列表
    /// </summary>
    public class TaskQueueList
    {
        /// <summary>
        /// 任务列表
        /// </summary>
        private static readonly Queue<TaskVO> _queue = new();

        /// <summary>
        /// 将任务添加到队列中
        /// </summary>
        public static void Enqueue(List<TaskVO> lstTask)
        {
            foreach (var task in lstTask)
            {
                _queue.Enqueue(task);
            }
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        public static void RunJob()
        {
            // 开启任务，自动拉取任务
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // 没有任务的时候，要主动拉取
                    if (_queue.Count == 0)
                    {
                        // 拉取任务
                        await TaskManager.PullAsync();
                    }

                    if (_queue.Count == 0)
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    // 计划时间还没到
                    var waitTimeSpan = _queue.Peek().StartAt - DateTime.Now;
                    if (waitTimeSpan.TotalMilliseconds > 0)
                    {
                        Thread.Sleep(waitTimeSpan);
                    }

                    // 取出任务
                    RunTask(_queue.Dequeue());

                    Thread.Sleep(500);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static async Task RunTask(TaskVO task)
        {
            var message = $"任务组：TaskGroupId={task.TaskGroupId}，Caption={task.Caption}，JobName={task.JobName}，TaskId={task.Id}";

            // JOB执行耗时计数器
            var sw = new Stopwatch();
            // 上下文
            var receiveContext = new ReceiveContext(task, sw);
            var cts            = new CancellationTokenSource();

            // 开启激活任务，直到任务完成
            Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await receiveContext.ActivateTask();
                    await Task.Delay(10000, cts.Token);
                }
            }, cts.Token);

            // 业务是否有该调度任务的实现
            var isRegistered = IocManager.Instance.IsRegistered($"fss_job_{task.JobName}");
            if (isRegistered is false)
            {
                IocManager.Instance.Logger<TaskQueueList>().LogWarning($"未找到任务实现类：{message}");
                await receiveContext.FailAsync(new LogRequest()
                {
                    LogLevel = LogLevel.Error,
                    Log      = $"未找到任务实现类：{message}",
                    CreateAt = DateTime.Now
                });
                return;
            }

            try
            {
                try
                {
                    // 执行业务JOB
                    var fssJob = IocManager.Instance.Resolve<IFssJob>($"fss_job_{task.JobName}");
                    sw.Start();
                    bool result;
                    using (FsLinkTrack.TrackFss(task.ClientHost, task.JobName, task.TaskGroupId, task.Id))
                    {
                        result = await fssJob.Execute(receiveContext);
                    }

                    // 写入链路追踪
                    if (IocManager.Instance.IsRegistered<ILinkTrackQueue>()) IocManager.Instance.Resolve<ILinkTrackQueue>().Enqueue();

                    // 通知服务端，当前客户端执行结果
                    if (result) await receiveContext.SuccessAsync();
                    else await receiveContext.FailAsync();
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<TaskQueueList>().LogError(e, $"taskGroupId={receiveContext.Meta.TaskGroupId},caption={receiveContext.Meta.Caption}：{e.Message}");
                    await receiveContext.LoggerAsync(LogLevel.Error, e.ToString());
                }
                finally
                {
                    sw.Stop();
                    cts.Cancel();
                }
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<TaskQueueList>().LogError(e.Message);
            }
        }
    }
}