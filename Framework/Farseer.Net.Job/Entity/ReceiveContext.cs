using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.Job.Entity
{
    /// <summary>
    ///     任务接收的上下文
    /// </summary>
    public class ReceiveContext
    {
        /// <summary>
        ///     是否为本地Debug模式
        /// </summary>
        private readonly bool _isDebug;

        private readonly Stopwatch   _sw;
        private          long        _nextTimespan;
        private          TimeSpan?   _ts;
        private          EumTaskType TaskStatus;

        public ReceiveContext(TaskVO task, Stopwatch sw)
        {
            Meta       =   task;
            _sw        =   sw;
            Meta.Data  ??= new Dictionary<string, string>();
            TaskStatus =   EumTaskType.Working;
        }

        /// <summary>
        ///     DEBUG模式
        /// </summary>
        internal ReceiveContext(IIocManager ioc, Stopwatch sw, Dictionary<string, string> debugMetaData)
        {
            Meta = new TaskVO
            {
                Caption = "调试",
                JobName = null,
                Data    = debugMetaData
            };

            _isDebug   = true;
            _sw        = sw;
            TaskStatus = EumTaskType.Working;
        }

        internal int Progress { get; set; }

        /// <summary>
        ///     任务组的参数
        /// </summary>
        public TaskVO Meta { get; }

        /// <summary>
        ///     返回进度0-100
        /// </summary>
        public Task SetProgressAsync(int rate)
        {
            if (rate is < 0 or > 100) throw new Exception(message: "ReceiveContext.SetProgress的rate只能是0-100");
            Progress = rate;
            return ActivateTask();
        }

        /// <summary>
        ///     本次执行完后，下一次执行的间隔时间
        /// </summary>
        public void SetNextAt(TimeSpan ts)
        {
            _ts = ts;
        }

        /// <summary>
        ///     写入到FSS平台的日志
        /// </summary>
        public async Task LoggerAsync(LogLevel logLevel, string log)
        {
            IocManager.Instance.Logger<ReceiveContext>().Log(logLevel: logLevel, message: log);
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    Id           = Meta.Id,
                    NextTimespan = _nextTimespan,
                    Progress     = Progress,
                    Status       = TaskStatus,
                    RunSpeed     = (int)_sw.ElapsedMilliseconds,
                    Log = new LogRequest
                    {
                        LogLevel = logLevel,
                        Log      = log,
                        CreateAt = DateTime.Now
                    },
                    Data = Meta.Data
                });
            }
        }

        /// <summary>
        ///     成功后执行
        /// </summary>
        internal async Task SuccessAsync(LogRequest log = null)
        {
            // 如果本次有动态设计时间
            if (_ts.HasValue) _nextTimespan = (int)_ts.GetValueOrDefault().TotalMilliseconds;
            TaskStatus = EumTaskType.Success;
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    Id           = Meta.Id,
                    NextTimespan = _nextTimespan,
                    Progress     = 100,
                    Status       = TaskStatus,
                    RunSpeed     = (int)_sw.ElapsedMilliseconds,
                    Log          = log,
                    Data         = Meta.Data
                });
            }
        }

        /// <summary>
        ///     执行失败
        /// </summary>
        public async Task FailAsync(LogRequest log = null)
        {
            // 如果本次有动态设计时间
            if (_ts.HasValue) _nextTimespan = (int)_ts.GetValueOrDefault().TotalMilliseconds;
            TaskStatus = EumTaskType.Fail;
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    Id           = Meta.Id,
                    NextTimespan = _nextTimespan,
                    Progress     = Progress,
                    Status       = TaskStatus,
                    RunSpeed     = (int)_sw.ElapsedMilliseconds,
                    Log          = log,
                    Data         = Meta.Data
                });
            }
        }

        /// <summary>
        ///     激活任务
        /// </summary>
        public async Task ActivateTask()
        {
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    Id           = Meta.Id,
                    NextTimespan = _nextTimespan,
                    Progress     = Progress,
                    Status       = TaskStatus,
                    RunSpeed     = (int)_sw.ElapsedMilliseconds,
                    Log          = null,
                    Data         = Meta.Data
                });
            }
        }
    }
}