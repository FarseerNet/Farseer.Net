using System;
using FS.Configs;
using FS.Log.Default.Entity;
using FS.Utils.Common;

namespace FS.Log.Default
{
    public class DefaultLog : ILog
    {
        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite DebugLogWrite = new LogWrite(EumLogType.Debug, SysMapPath.DebugPath, 5);
        public void Debug(string message) => Debug(null, message);
        public void Debug(Exception exception, string message = null)
        {
            if (SystemConfigs.ConfigEntity.IsWriteDebugLog) { DebugLogWrite.Write(message, exception); }
        }

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite ErrorLogWrite = new LogWrite(EumLogType.Error, SysMapPath.ErrorPath, 5);
        public void Error(string message) => Error(null, message);
        public void Error(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteErrorLog) { ErrorLogWrite.Write(message, exception); }
        }

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite FatalLogWrite = new LogWrite(EumLogType.Fatal, SysMapPath.FatalPath, 5);
        public void Fatal(string message) => Fatal(null, message);
        public void Fatal(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteFatalLog) { FatalLogWrite.Write(message, exception); }
        }

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite InfoLogWrite = new LogWrite(EumLogType.Info, SysMapPath.InfoPath, 5);
        public void Info(string message) => Info(null, message);
        public void Info(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteInfoLog) { InfoLogWrite.Write(message, exception); }
        }

        /// <summary>
        /// 调试日志写入器
        /// </summary>
        private static readonly LogWrite WarnLogWrite = new LogWrite(EumLogType.Warn, SysMapPath.WarnPath, 5);
        public void Warn(string message) => Warn(null, message);
        public void Warn(Exception exception, string message = null)
        {
            if (SystemConfigs.ConfigEntity.IsWriteWarnLog) { WarnLogWrite.Write(message, exception); }
        }
    }
}
