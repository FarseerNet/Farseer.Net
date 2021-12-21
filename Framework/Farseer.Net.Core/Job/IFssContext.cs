using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FS.Core.Job
{
    public interface IFssContext
    {
        /// <summary>
        ///     返回进度0-100
        /// </summary>
        void SetProgress(int rate);
        /// <summary>
        ///     本次执行完后，下一次执行的间隔时间
        /// </summary>
        void SetNextAt(TimeSpan ts);
        /// <summary>
        ///     本次执行完后，下一次执行的间隔时间
        /// </summary>
        void SetNextAt(DateTime dt);
        /// <summary>
        ///     写入到FSS平台的日志
        /// </summary>
        Task LoggerAsync(LogLevel logLevel, string log);
        /// <summary>
        ///     激活任务
        /// </summary>
        Task ActivateTask();
        /// <summary>
        ///     任务组的参数
        /// </summary>
        TaskVO Meta { get; }
    }
}