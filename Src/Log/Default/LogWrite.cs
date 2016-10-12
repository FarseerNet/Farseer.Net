// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-10-12 10:49
// ********************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using FS.Cache;
using FS.Component;
using FS.Configs;
using FS.Log.Default.Entity;

namespace FS.Log.Default
{
    /// <summary>
    /// 日志持久化工具
    /// </summary>
    internal class LogWrite
    {
        /// <summary> 写入路径 </summary>
        private readonly string _filePath;
        /// <summary> 日志类型 </summary>
        private readonly EumLogType _logType;
        /// <summary> 延迟写入时间 </summary>
        private readonly int _lazyTime;
        /// <summary> SQL执行记录列表 </summary>
        private readonly List<object> _sqlLogList = new List<object>();
        /// <summary>
        /// 时间定时器
        /// </summary>
        private Timer _timerSave;

        /// <summary>
        /// 日志持久化工具
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="filePath">写入路径</param>
        /// <param name="lazyTime">延迟写入时间</param>
        internal LogWrite(EumLogType logType, string filePath, int lazyTime)
        {
            this._logType = logType;
            this._lazyTime = lazyTime;
            this._filePath = filePath;

            InitTimer();
        }

        /// <summary>
        /// 初始化定时器（持久化到日志文件的任务）
        /// </summary>
        private void InitTimer()
        {
            // 创建延迟执行
            _timerSave = new Timer(o =>
            {
                // 取得待持久化的数量
                var count = _sqlLogList.Count;
                if (count == 0) { return; }
                try
                {
                    System.IO.Directory.CreateDirectory(_filePath);
                    // 文件名
                    var fileName = $"{DateTime.Now:yy-MM-dd}.log";
                    // JSON序例化后保存的变量
                    var json = new string[count];
                    var jsoner = new DataContractJsonSerializer(_sqlLogList[0].GetType());
                    for (var i = 0; i < count; i++)
                    {
                        using (var stream = new MemoryStream())
                        {
                            // 序列化到流
                            jsoner.WriteObject(stream, _sqlLogList[i]);
                            // 存入到json数组
                            json[i] = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                    // 写入到日志文件
                    File.AppendAllLines(_filePath + fileName, json);
                    // 移除待持久化队列
                    _sqlLogList.RemoveRange(0, count);
                }
                catch (Exception)
                {
                    // ignored
                }
            }, null, 1000 * _lazyTime, 1000 * _lazyTime);   // 指定延迟时间写入数据
        }

        /// <summary>
        /// 将对象添加到待持久化队列
        /// </summary>
        internal void Add(object obj) => _sqlLogList.Add(obj);
        /// <summary>
        /// 添加日志消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="exp">异常</param>
        public void Write(string message, Exception exp) 
        {
            var log = new CommonLog { Exp = exp, Message = message ?? exp.Message.Replace("\r\n", "") };
            log.RecordExecuteMethod();
            //写入日志
            Add(log);
            // 发送邮件
            if (_logType == EumLogType.Error && SystemConfigs.ConfigEntity.IsSendErrorEmail) { SendEmail(log); }
        }

        /// <summary>
        /// 发送日志到邮件
        /// </summary>
        /// <param name="log"></param>
        private void SendEmail(CommonLog log)
        {
            var mail = ExceptionEmailConfigs.ConfigEntity;
            var smtp = new SmtpMail(mail.LoginName, mail.LoginPwd, mail.SendMail, $"Farseer.Net {EnumNameCacheManger.Cache(_logType)}记录", mail.SmtpServer, 0, mail.SmtpPort);
            var body = new StringBuilder();
            body.AppendFormat($"<b>发现时间：</b> {log.CreateAt:yyyy年MM月dd日 HH:mm:ss}<br />");
            body.AppendFormat($"<b>程序文件：</b> <u>{log.FileName}</u> <b>第{log.LineNo}行</b> <font color=red>{log.MethodName}()</font><br />");
            body.AppendFormat($"<b>日志消息：</b><font color=red>{0}</font><br />", log.Message);
            smtp.Send(mail.EmailAddress, $"{DateTime.Now:yyyy年MM月dd日 HH:mm:ss}：{EnumNameCacheManger.Cache(_logType)}消息：{log.Message}", body.ToString());
        }
    }
}