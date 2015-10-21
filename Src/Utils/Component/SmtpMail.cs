using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using FS.Configs;
using FS.Utils.Common;

namespace FS.Component
{
    /// <summary>
    ///     E-Mail工具
    /// </summary>
    public class SmtpMail
    {
        /// <summary> 登陆用户名 </summary>
        private readonly string _loginName;

        /// <summary> 登陆密码 </summary>
        private readonly string _loginPwd;

        /// <summary> 最多发件人数量 </summary>
        private readonly int _recipientMaxNum;

        /// <summary> 发件人E-Mail地址 </summary>
        private readonly string _sendMail;

        /// <summary> 发件人姓名 </summary>
        private readonly string _sendUserName;

        /// <summary> 端口号 </summary>
        private readonly int _smtpPort;

        /// <summary> 邮件服务器域名和验证信息     形如：Smtp.server.com" </summary>
        private readonly string _smtpServer;

        public SmtpMail(string loginName, string loginPwd, string sendMail, string sendUserName, string smtpServer, int recipientMaxNum, int smtpPort = 25)
        {
            _loginName = loginName;
            _loginPwd = loginPwd;
            _sendMail = sendMail;
            _sendUserName = sendUserName;
            _smtpServer = smtpServer;
            _recipientMaxNum = recipientMaxNum;
            _smtpPort = smtpPort;
        }

        /// <summary>
        ///     发送电子邮件
        /// </summary>
        /// <param name="lstAddress">收件人地址</param>
        /// <param name="subject">邮件的标题</param>
        /// <param name="body">邮件的正文</param>
        /// <param name="fileName">邮件附件路径名</param>
        public bool Send(List<string> lstAddress, string subject, string body, string fileName = "")
        {
            if (_recipientMaxNum > 0 && lstAddress.Count > _recipientMaxNum) { return false; }

            var objSmtpClient = new SmtpClient(_smtpServer, _smtpPort) {DeliveryMethod = SmtpDeliveryMethod.Network, Credentials = new NetworkCredential(_loginName, _loginPwd)};
            var objMailMessage = new MailMessage {From = new MailAddress(_sendMail, _sendUserName)};
            foreach (var item in lstAddress.Where(item => item.Contains("@"))) { objMailMessage.To.Add(new MailAddress(item)); }

            objMailMessage.Subject = subject; //发送邮件主题
            objMailMessage.Body = body; //发送邮件内容
            objMailMessage.BodyEncoding = Encoding.Default; //发送邮件正文编码
            objMailMessage.IsBodyHtml = true; //设置为HTML格式
            objMailMessage.Priority = MailPriority.High; //优先级
            if (!string.IsNullOrWhiteSpace(fileName)) objMailMessage.Attachments.Add(new Attachment(fileName)); //邮件的附件

            objSmtpClient.Send(objMailMessage);
            return true;
        }

        /// <summary>
        ///     发送电子邮件
        /// </summary>
        /// <param name="emailAddress">收件人地址，多个用;分隔</param>
        /// <param name="subject">邮件的标题</param>
        /// <param name="body">邮件的正文</param>
        /// <param name="fileName">邮件附件路径名</param>
        public bool Send(string emailAddress, string subject, string body, string fileName = "")
        {
            return Send(ConvertHelper.ToList(emailAddress, "", ";"), subject, body, fileName);
        }

        /// <summary>
        ///     发送电子邮件
        /// </summary>
        /// <param name="mail">Email配置</param>
        /// <param name="lstAddress">收件人地址</param>
        /// <param name="subject">邮件的标题</param>
        /// <param name="body">邮件的正文</param>
        /// <param name="fileName">邮件附件路径名</param>
        public static bool Send(List<string> lstAddress, string subject, string body, EmailInfo mail = null, string fileName = "")
        {
            if (mail == null) { mail = 0; }
            return new SmtpMail(mail.LoginName, mail.LoginPwd, mail.SendMail, mail.SendUserName, mail.SmtpServer, mail.RecipientMaxNum, mail.SmtpPort).Send(lstAddress, subject, body, fileName);
        }
    }
}