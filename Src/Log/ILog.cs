using System;
using System.Diagnostics;

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
        void Debug(string message);

        /// <summary>
        ///     调试信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Debug(Exception exception, string message = null);

        /// <summary>
        ///     错误信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Error(string message);

        /// <summary>
        ///     错误信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Error(Exception exception, string message = null);

        /// <summary>
        ///     严重信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Fatal(string message);

        /// <summary>
        ///     严重信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Fatal(Exception exception, string message = null);

        /// <summary>
        ///     一般信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Info(string message);

        /// <summary>
        ///     一般信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Info(Exception exception, string message = null);

        /// <summary>
        ///     警告信息
        /// </summary>
        /// <param name="message">信息内容</param>
        void Warn(string message);

        /// <summary>
        ///     警告信息
        /// </summary>
        /// <param name="message">信息内容</param>
        /// <param name="exception">异常类</param>
        void Warn(Exception exception, string message = null);
    }
}