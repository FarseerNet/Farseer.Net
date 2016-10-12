using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using FS.Component;
using FS.Configs;
using FS.Log.Default.Entity;
using FS.Utils.Common;

namespace FS.Log.Default
{
    /// <summary> SQL异常记录 </summary>
    [DataContract]
    public class SqlErrorLog : CommonLog
    {
        /// <summary>
        /// 日志写入器
        /// </summary>
        private static readonly LogWrite Writer = new LogWrite(EumLogType.Error, SysMapPath.SqlErrorPath, 5);

        /// <summary> 执行对象 </summary>
        [DataMember]
        public CommandType CmdType { get; set; }

        /// <summary> 执行表名称 </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary> 执行SQL </summary>
        [DataMember]
        public string Sql { get; set; }

        /// <summary> 执行参数 </summary>
        [DataMember]
        public List<SqlParam> SqlParamList { get; set; }

        /// <summary> SQL异常记录写入 </summary>
        /// <param name="name">表名称</param>
        /// <param name="cmdType">执行方式</param>
        /// <param name="sql">T-SQL</param>
        /// <param name="param">SQL参数</param>
        /// <param name="exp">异常信息</param>
        public SqlErrorLog(Exception exp, string name, CommandType cmdType, string sql, List<DbParameter> param)
        {
            Exp = exp;
            Message = exp.Message.Replace("\r\n", "");
            Name = name;
            CmdType = cmdType;
            Sql = sql;
            if (param != null && param.Count > 0)
            {
                SqlParamList = new List<SqlParam>();
                foreach (var t in param)
                {
                    SqlParamList.Add(new SqlParam { Name = t.ParameterName, Value = (t.Value ?? "null").ToString() });
                }
            }
            RecordExecuteMethod();
        }

        /// <summary> 发送邮件 </summary>
        private void SendEmail()
        {
            var mail = ExceptionEmailConfigs.ConfigEntity;
            var smtp = new SmtpMail(mail.LoginName, mail.LoginPwd, mail.SendMail, "Farseer.Net SQL异常记录", mail.SmtpServer, 0, mail.SmtpPort);
            var body = new StringBuilder();
            body.AppendFormat("<b>发现时间：</b> {0}<br />", CreateAt);
            body.AppendFormat("<b>程序文件：</b> <u>{0}</u> <b>第{1}行</b> <font color=red>{2}()</font><br />", FileName, LineNo, MethodName);

            switch (CmdType)
            {
                case CommandType.StoredProcedure:
                    body.AppendFormat("<b>存储过程：</b> {0}<br />", Name);
                    break;
                case CommandType.Text:
                    body.AppendFormat("<b>表视图名：</b> {0}<br />", Name);
                    body.AppendFormat("<b>Sql语句：</b> {0}<br />", Sql);
                    break;
            }

            body.AppendFormat("<b>Sql参数：</b><br />");
            SqlParamList.ForEach(o => body.AppendFormat("{0} = {1}<br />", o.Name, o.Value));
            body.AppendFormat("<b>错误消息：</b><font color=red>{0}</font><br />", Message);
            smtp.Send(mail.EmailAddress, $"{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}：警告！数据库异常：{Message}", body.ToString());
        }

        public void Write()
        {
            Writer.Add(this);
            // 发送邮件
            if (SystemConfigs.ConfigEntity.IsSendErrorEmail) { SendEmail(); }
        }
    }
}