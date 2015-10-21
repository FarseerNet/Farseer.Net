using System;
using System.Text;
using FS.Component;
using FS.Configs;
using FS.Utils.Common;

namespace FS.Log
{
    /// <summary> 运行异常记录 </summary>
    [Serializable]
    public class RunExceptionLogEntity : AbsLogEntity
    {
        /// <summary> 运行异常记录管理器 </summary>
        private static readonly CommonLogManger<RunExceptionLogEntity> LogManger = new CommonLogManger<RunExceptionLogEntity>(SysMapPath.AppData + "log/RunExceptionLog/", $"{DateTime.Now.ToString("yy-MM-dd")}.xml", 1);

        public RunExceptionLogEntity()
        {
        }

        /// <summary> 运行异常记录写入~/App_Data/RunExceptionLog.xml </summary>
        /// <param name="ex">异常信息</param>
        public static void Write(Exception ex)
        {
            var entity = new RunExceptionLogEntity {ex = ex, Message = ex.Message.Replace("\r\n", ""),};
            entity.Write();
        }

        /// <summary> 异常消息 </summary>
        public string Message { get; set; }

        /// <summary> 写入~/App_Data/RunExceptionLog.xml </summary>
        private void Write()
        {
            RecordExecuteMethod();

            LogManger.Write(this);

            // 发送邮件
            if (SystemConfigs.ConfigEntity.IsWriteRunExceptionLog) { SendEmail(); }
        }

        private void SendEmail()
        {
            var mail = ExceptionEmailConfigs.ConfigEntity;
            var smtp = new SmtpMail(mail.LoginName, mail.LoginPwd, mail.SendMail, "Farseer.Net 运行异常记录", mail.SmtpServer, 0, mail.SmtpPort);
            var body = new StringBuilder();
            body.AppendFormat("<b>发现时间：</b> {0}<br />", CreateAt.ToString("yyyy年MM月dd日 HH:mm:ss"));
            body.AppendFormat("<b>程序文件：</b> <u>{0}</u> <b>第{1}行</b> <font color=red>{2}()</font><br />", FileName, LineNo, MethodName);
            body.AppendFormat("<b>错误消息：</b><font color=red>{0}</font><br />", Message);
            smtp.Send(mail.EmailAddress, $"{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}：警告！数据库异常：{Message}", body.ToString());
        }
    }
}