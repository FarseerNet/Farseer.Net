using System;
using System.Text;
using FS.Component;
using FS.Configs;
using FS.Utils.Common;

namespace FS.Log.Entity
{
    /// <summary> 运行记录 </summary>
    [Serializable]
    public class LogEntity : AbsLogEntity<LogEntity>
    {
        public LogEntity() : base(eumLogType.Debug, null, null, 0) { }
        public LogEntity(eumLogType logType, string message, Exception exp) : base(logType, SysMapPath.ErrorPath, $"{DateTime.Now.ToString("yy-MM-dd")}.xml", 1)
        {
            Exp = exp;
            Message = message ?? exp.Message.Replace("\r\n", "");
        }

        public override void AddToQueue()
        {
            //写入日志
            AddToQueue(this);

            // 发送邮件
            if (logType == eumLogType.Error && SystemConfigs.ConfigEntity.IsSendExceptionEMail) { SendEmail(); }
        }
    }
}