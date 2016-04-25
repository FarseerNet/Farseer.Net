using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using FS.Configs;
using FS.Log.Entity;
using FS.Utils.Common;

namespace FS.Log
{
    public class DefaultLog : ILog
    {
        public void Debug(string message)
        {
            Debug(null, message);
        }

        public void Debug(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteRunExceptionLog) { new LogEntity(eumLogType.Debug, message, exception).AddToQueue(); }
        }

        public void Error(string message)
        {
            Error(null, message);
        }

        public void Error(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteRunExceptionLog) { new LogEntity(eumLogType.Error, message, exception).AddToQueue(); }
        }

        public void Fatal(string message)
        {
            Fatal(null, message);
        }

        public void Fatal(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteRunExceptionLog) { new LogEntity(eumLogType.Fatal, message, exception).AddToQueue(); }
        }

        public void Info(string message)
        {
            Info(null, message);
        }

        public void Info(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteRunExceptionLog) { new LogEntity(eumLogType.Info, message, exception).AddToQueue(); }
        }

        public void Warn(string message)
        {
            Warn(null, message);
        }

        public void Warn(Exception exception, string message = null)
        {
            //写入日志
            if (SystemConfigs.ConfigEntity.IsWriteRunExceptionLog) { new LogEntity(eumLogType.Warn, message, exception).AddToQueue(); }
        }
    }
}
