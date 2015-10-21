using System;

namespace FS.Configs
{
    /// <summary> 数据库异常邮件发送参数 </summary>
    public class ExceptionEmailConfigs : AbsConfigs<ExceptionEmailConfig>
    {
    }

    /// <summary> 数据库异常邮件发送参数 </summary>
    [Serializable]
    public class ExceptionEmailConfig
    {
        /// <summary> 登陆用户名 </summary>
        public string LoginName = "登陆用户名";

        /// <summary> 登陆密码 </summary>
        public string LoginPwd = "登陆密码";

        /// <summary> 发件人E-Mail地址 </summary>
        public string SendMail = "发件人E-Mail地址";

        /// <summary> 端口号 </summary>
        public int SmtpPort = 25;

        /// <summary> 邮件服务器域名和验证信息     形如：Smtp.server.com" </summary>
        public string SmtpServer = "邮件服务器域名和验证信息";

        /// <summary> 通知人邮箱地址，多个用;分隔</summary>
        public string EmailAddress = "通知人邮箱地址，多个用;分隔";
    }
}