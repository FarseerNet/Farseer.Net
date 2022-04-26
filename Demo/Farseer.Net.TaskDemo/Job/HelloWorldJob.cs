using FS.Core.Job;
using FS.Tasks;
using Microsoft.Extensions.Logging;

namespace Farseer.Net.TaskDemo.Job
{
    [Job(Interval = 200)] // 需要附加Job特性，并设置执行间隔
    public class HelloWorldJob : IJob
    {
        /// <summary>
        ///     执行任务
        /// </summary>
        public Task Execute(ITaskContext context)
        {
            // 让FSS平台，记录日志
            context.Logger(logLevel: LogLevel.Information, log: "你好，世界！");

            context.SetNext(TimeSpan.FromSeconds(5));
            // 任务执行成功
            return Task.FromResult(0);
        }
    }
}