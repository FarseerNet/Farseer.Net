using System;
using System.Collections.Generic;

namespace FS.Configs
{
    /// <summary> Email配置信息 </summary>
    public class EmailConfigs : AbsConfigs<EmailConfig>
    {
    }

    /// <summary> Email配置信息 </summary>
    [Serializable]
    public class EmailConfig
    {
        /// <summary> Email配置列表 </summary>
        public List<EmailInfo> EmailList = new List<EmailInfo>();
    }

    /// <summary> Email配置信息 </summary>
    public class EmailInfo
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

        /// <summary> 最多发件人数量 </summary>
        public int RecipientMaxNum = 5;

        /// <summary> 发件人姓名 </summary>
        public string SendUserName = "发件人姓名";

        /// <summary> 通过索引返回实体 </summary>
        public static implicit operator EmailInfo(int index)
        {
            return EmailConfigs.ConfigEntity.EmailList.Count <= index ? null : EmailConfigs.ConfigEntity.EmailList[index];
        }
    }
}