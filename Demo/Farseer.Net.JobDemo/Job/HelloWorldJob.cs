using System.Threading.Tasks;
using FS.Core.Job;
using FS.Job;
using FS.Job.Entity;
using Microsoft.Extensions.Logging;

namespace Farseer.Net.JobDemo.Job
{
    [FssJob(Name = "testJob")] // Name与FSS平台配置的JobName保持一致
    public class HelloWorldJob : IFssJob
    {
        /// <summary>
        ///     执行任务
        /// </summary>
        public async Task<bool> Execute(IFssContext context)
        {
            // 告诉FSS平台，当前进度执行了 20%
            await context.SetProgressAsync(rate: 20);

            // 让FSS平台，记录日志
            await context.LoggerAsync(logLevel: LogLevel.Information, log: "你好，世界！");

            // 下一次执行时间为10秒后（如果不设置，则使用任务组设置的时间）
            //context.SetNextAt(TimeSpan.FromSeconds(10));

            // 任务执行成功
            return true;
        }
    }
}