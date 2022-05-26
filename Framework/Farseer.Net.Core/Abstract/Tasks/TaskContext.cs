using System;
using System.Diagnostics;
using FS.Core.Abstract.Fss;
using FS.DI;
using FS.Extends;
using Microsoft.Extensions.Logging;

namespace FS.Core.Abstract.Tasks;

/// <summary>
///     任务接收的上下文
/// </summary>
public class TaskContext : ITaskContext
{
    private readonly Type      _jobType;
    private readonly Stopwatch _sw;

    public TaskContext(Type jobType, Stopwatch sw)
    {
        _jobType = jobType;
        _sw      = sw;
    }
    public long NextTimespan { get; private set; }

    /// <summary>
    ///     本次执行完后，下一次执行的间隔时间
    /// </summary>
    public void SetNext(TimeSpan ts)
    {
        NextTimespan = DateTime.Now.Add(value: ts).ToTimestamps();
    }

    /// <summary>
    ///     本次执行完后，下一次执行的间隔时间
    /// </summary>
    public void SetNext(DateTime dt)
    {
        NextTimespan = dt.ToTimestamps();
    }

    /// <summary>
    ///     写入到FSS平台的日志
    /// </summary>
    public void Logger(LogLevel logLevel, string log)
    {
        IocManager.Instance.Logger<TaskContext>().Log(logLevel: logLevel, message: log);
    }
}