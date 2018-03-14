using System;
using FS.Configuration;

namespace FS.Data.Log.Default
{
    public class DefaultLog : ILog
    {
        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite DebugLogWrite = new LogWrite(EumLogType.Debug, SysPath.DebugPath, 5);
        public void Debug(string message) => Debug(null, message);
        public void Debug(Exception exception, string message = null) => DebugLogWrite.Write(message, exception);

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite ErrorLogWrite = new LogWrite(EumLogType.Error, SysPath.ErrorPath, 5);
        public void Error(string message) => Error(null, message);
        public void Error(Exception exception, string message = null) => ErrorLogWrite.Write(message, exception);

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite FatalLogWrite = new LogWrite(EumLogType.Fatal, SysPath.FatalPath, 5);
        public void Fatal(string message) => Fatal(null, message);
        public void Fatal(Exception exception, string message = null) => FatalLogWrite.Write(message, exception);

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite InfoLogWrite = new LogWrite(EumLogType.Info, SysPath.InfoPath, 5);
        public void Info(string message) => Info(null, message);
        public void Info(Exception exception, string message = null) => InfoLogWrite.Write(message, exception);

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite WarnLogWrite = new LogWrite(EumLogType.Warn, SysPath.WarnPath, 5);
        public void Warn(string message) => Warn(null, message);
        public void Warn(Exception exception, string message = null) => WarnLogWrite.Write(message, exception);
    }
}
