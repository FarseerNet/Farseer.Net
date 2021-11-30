using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.Core.Job;
using FS.DI;
using FS.Extends;
using Microsoft.Extensions.Logging;

namespace FS.Job.Entity
{
    /// <summary>
    ///     任务接收的上下文
    /// </summary>
    public class FssContext : IFssContext
    {
        /// <summary>
        ///     是否为本地Debug模式
        /// </summary>
        private readonly bool _isDebug;

        private readonly Stopwatch   _sw;
        private          long        _nextTimespan;
        private          EumTaskType _taskStatus;

        public FssContext(TaskVO task, Stopwatch sw)
        {
            Meta        =   task;
            _sw         =   sw;
            Meta.Data   ??= new Dictionary<string, string>();
            _taskStatus =   EumTaskType.Working;
        }

        /// <summary>
        ///     DEBUG模式
        /// </summary>
        internal FssContext(IIocManager ioc, string jobName, Stopwatch sw, Dictionary<string, string> debugMetaData)
        {
            Meta = new TaskVO
            {
                Caption = "调试",
                JobName = jobName,
                Data    = debugMetaData ?? new Dictionary<string, string>()
            };

            _isDebug    = true;
            _sw         = sw;
            _taskStatus = EumTaskType.Working;
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
            _nextTimespan = DateTime.Now.Add(ts).ToTimestamps();
        }

        /// <summary>
        ///     本次执行完后，下一次执行的间隔时间
        /// </summary>
        public void SetNextAt(DateTime dt)
        {
            _nextTimespan = dt.ToTimestamps();
        }

        /// <summary>
        ///     写入到FSS平台的日志
        /// </summary>
        public async Task LoggerAsync(LogLevel logLevel, string log)
        {
            IocManager.Instance.Logger<FssContext>().Log(logLevel: logLevel, message: log);
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    NextTimespan = _nextTimespan,
                    Progress     = Progress,
                    Status       = _taskStatus,
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
            _taskStatus = EumTaskType.Success;
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    NextTimespan = _nextTimespan,
                    Progress     = 100,
                    Status       = _taskStatus,
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
            _taskStatus = EumTaskType.Fail;
            if (!_isDebug)
            {
                await TaskManager.JobInvokeAsync(request: new JobInvokeRequest
                {
                    TaskGroupId  = Meta.TaskGroupId,
                    NextTimespan = _nextTimespan,
                    Progress     = Progress,
                    Status       = _taskStatus,
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
                    NextTimespan = _nextTimespan,
                    Progress     = Progress,
                    Status       = _taskStatus,
                    RunSpeed     = (int)_sw.ElapsedMilliseconds,
                    Log          = null,
                    Data         = Meta.Data
                });
            }
        }
    }
}