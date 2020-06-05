using System;
using Castle.Core.Logging;

namespace FS
{
    public class DefaultLogger : ILogger
    {
        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled  { get; }
        public bool IsWarnEnabled  { get; }

        public ILogger CreateChildLogger(string loggerName) => this;
        public void Trace(string message) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");

        public void Trace(Func<string> messageFactory) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {messageFactory()}");

        public void Trace(string message, Exception exception) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {exception.ToString()}");

        public void TraceFormat(string format, params object[] args) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format}", args);

        public void TraceFormat(Exception exception, string format, params object[] args) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format} {exception.ToString()}", args);

        public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)}", args);

        public void TraceFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)} {exception.ToString()}", args);

        public void Debug(string message) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");

        public void Debug(Func<string> messageFactory) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {messageFactory()}");

        public void Debug(string message, Exception exception) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {exception.ToString()}");

        public void DebugFormat(string format, params object[] args) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format}", args);

        public void DebugFormat(Exception exception, string format, params object[] args) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format} {exception.ToString()}", args);

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)}", args);

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)} {exception.ToString()}", args);


        public void Error(string message) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");

        public void Error(Func<string> messageFactory) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {messageFactory()}");

        public void Error(string message, Exception exception) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {exception.ToString()}");

        public void ErrorFormat(string format, params object[] args) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format}", args);

        public void ErrorFormat(Exception exception, string format, params object[] args) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format} {exception.ToString()}", args);

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)}", args);

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)} {exception.ToString()}", args);


        public void Fatal(string message) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");

        public void Fatal(Func<string> messageFactory) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {messageFactory()}");

        public void Fatal(string message, Exception exception) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {exception.ToString()}");

        public void FatalFormat(string format, params object[] args) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format}", args);

        public void FatalFormat(Exception exception, string format, params object[] args) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format} {exception.ToString()}", args);

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)}", args);

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)} {exception.ToString()}", args);


        public void Info(string message) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");

        public void Info(Func<string> messageFactory) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {messageFactory()}");

        public void Info(string message, Exception exception) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {exception.ToString()}");

        public void InfoFormat(string format, params object[] args) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format}", args);

        public void InfoFormat(Exception exception, string format, params object[] args) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format} {exception.ToString()}", args);

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)}", args);

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)} {exception.ToString()}", args);


        public void Warn(string message) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");

        public void Warn(Func<string> messageFactory) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {messageFactory()}");

        public void Warn(string message, Exception exception) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {exception.ToString()}");

        public void WarnFormat(string format, params object[] args) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format}", args);

        public void WarnFormat(Exception exception, string format, params object[] args) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format} {exception.ToString()}", args);

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)}", args);

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args) => Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {format.ToString(formatProvider)} {exception.ToString()}", args);
        public bool IsTraceEnabled { get; }
    }
}