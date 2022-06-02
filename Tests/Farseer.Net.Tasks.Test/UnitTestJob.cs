using System;
using System.Threading.Tasks;
using FS;
using FS.Core.Abstract.Fss;
using FS.Core.Abstract.Tasks;
using FS.Tasks;
using Microsoft.Extensions.Logging;

namespace Farseer.Net.Tasks.Test
{
    [Job(Interval = 200)] // 需要附加Job特性，并设置执行间隔
    public class UnitTestJob : IJob
    {
        public static long     ID;
        public static long     Interval;
        public static DateTime LastAt;

        /// <summary>
        ///     执行任务
        /// </summary>
        public Task Execute(ITaskContext context)
        {
            if (ID == 0)
            {
                ID = FarseerApplication.AppId;
            }
            else
            {
                Interval = (long)(DateTime.Now - LastAt).TotalMilliseconds;
            }

            LastAt = DateTime.Now;

            // 任务执行成功
            return Task.FromResult(0);
        }
    }
}