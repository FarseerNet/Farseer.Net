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
        public bool IsTraceEnabled { get; }

        public DefaultLogger()
        {
            IsDebugEnabled = !Env.IsPro;
            IsInfoEnabled = true;
            IsErrorEnabled = true;
            IsFatalEnabled = true;
            IsWarnEnabled = true;
            IsTraceEnabled = true;
        }
        
        public ILogger CreateChildLogger(string loggerName) => this;

        public void Trace(string message)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"【Trace】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }

        public void Trace(Func<string>          messageFactory)                                                                               => Trace(messageFactory());
        public void Trace(string                message,        Exception       exception)                                                    => Trace($"{message} {exception.ToString()}");
        public void TraceFormat(string          format,         params object[] args)                                                         => Trace(string.Format(format, args));
        public void TraceFormat(Exception       exception,      string          format,         params object[] args)                         => Trace($"{string.Format(format, args)} {exception}");
        public void TraceFormat(IFormatProvider formatProvider, string          format,         params object[] args)                         => Trace(string.Format(format.ToString(formatProvider), args));
        public void TraceFormat(Exception       exception,      IFormatProvider formatProvider, string          format, params object[] args) => Trace($"{string.Format(format.ToString(formatProvider), args)} {exception}");


        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"【Debug】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }

        public void Debug(Func<string>          messageFactory)                                                                               => Debug(messageFactory());
        public void Debug(string                message,        Exception       exception)                                                    => Debug($"{message} {exception.ToString()}");
        public void DebugFormat(string          format,         params object[] args)                                                         => Debug(string.Format(format, args));
        public void DebugFormat(Exception       exception,      string          format,         params object[] args)                         => Debug($"{string.Format(format, args)} {exception}");
        public void DebugFormat(IFormatProvider formatProvider, string          format,         params object[] args)                         => Debug(string.Format(format.ToString(formatProvider), args));
        public void DebugFormat(Exception       exception,      IFormatProvider formatProvider, string          format, params object[] args) => Debug($"{string.Format(format.ToString(formatProvider), args)} {exception}");


        public void Error(string                message)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"【Error】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }

        public void Error(Func<string>          messageFactory)                                                                               => Error(messageFactory());
        public void Error(string                message,        Exception       exception)                                                    => Error($"{message} {exception.ToString()}");
        public void ErrorFormat(string          format,         params object[] args)                                                         => Error(string.Format(format, args));
        public void ErrorFormat(Exception       exception,      string          format,         params object[] args)                         => Error($"{string.Format(format, args)} {exception}");
        public void ErrorFormat(IFormatProvider formatProvider, string          format,         params object[] args)                         => Error(string.Format(format.ToString(formatProvider), args));
        public void ErrorFormat(Exception       exception,      IFormatProvider formatProvider, string          format, params object[] args) => Error($"{string.Format(format.ToString(formatProvider), args)} {exception}");


        public void Fatal(string                message)
        {
            if (!IsFatalEnabled) return;
            Console.WriteLine($"【Fatal】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }

        public void Fatal(Func<string>          messageFactory)                                                                               => Fatal(messageFactory());
        public void Fatal(string                message,        Exception       exception)                                                    => Fatal($"{message} {exception.ToString()}");
        public void FatalFormat(string          format,         params object[] args)                                                         => Fatal(string.Format(format, args));
        public void FatalFormat(Exception       exception,      string          format,         params object[] args)                         => Fatal($"{string.Format(format, args)} {exception}");
        public void FatalFormat(IFormatProvider formatProvider, string          format,         params object[] args)                         => Fatal(string.Format(format.ToString(formatProvider), args));
        public void FatalFormat(Exception       exception,      IFormatProvider formatProvider, string          format, params object[] args) => Fatal($"{string.Format(format.ToString(formatProvider), args)} {exception}");


        public void Info(string                message)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"【Info】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }

        public void Info(Func<string>          messageFactory)                                                                               => Info(messageFactory());
        public void Info(string                message,        Exception       exception)                                                    => Info($"{message} {exception.ToString()}");
        public void InfoFormat(string          format,         params object[] args)                                                         => Info(string.Format(format, args));
        public void InfoFormat(Exception       exception,      string          format,         params object[] args)                         => Info($"{string.Format(format, args)} {exception}");
        public void InfoFormat(IFormatProvider formatProvider, string          format,         params object[] args)                         => Info(string.Format(format.ToString(formatProvider), args));
        public void InfoFormat(Exception       exception,      IFormatProvider formatProvider, string          format, params object[] args) => Info($"{string.Format(format.ToString(formatProvider), args)} {exception}");


        public void Warn(string                message)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"【Warn】{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }

        public void Warn(Func<string>          messageFactory)                                                                               => Warn(messageFactory());
        public void Warn(string                message,        Exception       exception)                                                    => Warn($"{message} {exception.ToString()}");
        public void WarnFormat(string          format,         params object[] args)                                                         => Warn(string.Format(format, args));
        public void WarnFormat(Exception       exception,      string          format,         params object[] args)                         => Warn($"{string.Format(format, args)} {exception}");
        public void WarnFormat(IFormatProvider formatProvider, string          format,         params object[] args)                         => Warn(string.Format(format.ToString(formatProvider), args));
        public void WarnFormat(Exception       exception,      IFormatProvider formatProvider, string          format, params object[] args) => Warn($"{string.Format(format.ToString(formatProvider), args)} {exception}");
    }
}