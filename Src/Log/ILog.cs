using System;

namespace FS.Log
{
    /// <summary>
    ///     日志接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        ///     调试信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Debug(object message);

        /// <summary>
        ///     调试信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Debug(object message, Exception exception);

        /// <summary>
        ///     调试信息
        /// </summary>
        /// <param name="format">信息内容</param>
        /// <param name="args">参数</param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        ///     错误信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Error(object message);

        /// <summary>
        ///     错误信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Error(object message, Exception exception);

        /// <summary>
        ///     错误信息
        /// </summary>
        /// <param name="format">信息内容</param>
        /// <param name="args">参数</param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        ///     严重信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Fatal(object message);

        /// <summary>
        ///     严重信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Fatal(object message, Exception exception);

        /// <summary>
        ///     严重信息
        /// </summary>
        /// <param name="format">信息内容</param>
        /// <param name="args">参数</param>
        void FatalFormat(string format, params object[] args);

        /// <summary>
        ///     一般信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Info(object message);

        /// <summary>
        ///     一般信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Info(object message, Exception exception);

        /// <summary>
        ///     一般信息
        /// </summary>
        /// <param name="format">信息内容</param>
        /// <param name="args">参数</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        ///     警告信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Warn(object message);

        /// <summary>
        ///     警告信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Warn(object message, Exception exception);

        /// <summary>
        ///     警告信息
        /// </summary>
        /// <param name="format">信息内容</param>
        /// <param name="args">参数</param>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        ///     是否为调试状态
        /// </summary>
        bool IsDebugEnabled { get; }
    }
}