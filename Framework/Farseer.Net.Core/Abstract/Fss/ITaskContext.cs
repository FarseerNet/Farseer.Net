using System;
using Microsoft.Extensions.Logging;

namespace FS.Core.Abstract.Fss
{
    public interface ITaskContext
    {
        /// <summary>
        ///     本次执行完后，下一次执行的间隔时间
        /// </summary>
        void SetNext(TimeSpan ts);
        /// <summary>
        ///     本次执行完后，下一次执行的间隔时间
        /// </summary>
        void SetNext(DateTime dt);
        /// <summary>
        ///     写入到FSS平台的日志
        /// </summary>
        void Logger(LogLevel logLevel, string log);
    }
}