using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using FS.Cache;
using FS.Component;
using FS.Configs;
using FS.Utils.Common;
using FS.Extends;

namespace FS.Log.Entity
{
    /// <summary>
    ///     日志记录
    /// </summary>
    public abstract class AbsLogEntity<TEntity>
    {
        protected AbsLogEntity(eumLogType logType, string filePath, string fileName, int lazyTime)
        {
            this.logType = logType;
            this._lazyTime = lazyTime;
            this._filePath = filePath;
            this._fileName = fileName;
        }

        private readonly string _filePath;
        private readonly string _fileName;
        public readonly eumLogType logType;
        private readonly int _lazyTime;

        /// <summary> SQL日志保存定时器/// </summary>
        private static readonly Dictionary<Type, Timer> SaveSqlRecord = new Dictionary<Type, Timer>();
        /// <summary> 线程锁 </summary>
        private static readonly object LockObject = new object();
        /// <summary> SQL执行记录列表 </summary>
        private static readonly List<TEntity> SqlLogList = new List<TEntity>();

        /// <summary> 执行行数 </summary>
        public int LineNo { get; set; }

        /// <summary> 执行方法名称 </summary>
        public string MethodName { get; set; }

        /// <summary> 执行方法的文件名 </summary>
        public string FileName { get; set; }

        /// <summary> 执行时间 </summary>
        public DateTime CreateAt { get; set; }

        /// <summary> 执行时间 </summary>
        protected Exception Exp { get; set; }

        /// <summary> 异常消息 </summary>
        public string Message { get; set; }

        /// <summary>
        ///     记录执行时的方法及文件
        /// </summary>
        protected void RecordExecuteMethod()
        {
            CreateAt = DateTime.Now;

            var lstFrames = Exp == null ? new StackTrace(true).GetFrames() : new StackTrace(Exp, true).GetFrames();
            var stack = lstFrames?.LastOrDefault(o => o.GetFileLineNumber() != 0 && !ConvertHelper.IsEquals(o.GetMethod().Module.Name, "Farseer.Net.dll") && !ConvertHelper.IsEquals(o.GetMethod().Name, "Callback"));
            if (stack == null) return;

            LineNo = stack.GetFileLineNumber();
            MethodName = stack.GetMethod().Name;
            FileName = stack.GetFileName();
        }

        /// <summary>
        ///     指定延迟时间写入数据
        /// </summary>
        protected void AddToQueue(TEntity entity)
        {
            RecordExecuteMethod();

            SqlLogList.Add(entity);
            // 已创建延迟时，退出
            if (SaveSqlRecord.ContainsKey(typeof(TEntity)) && SaveSqlRecord[typeof(TEntity)] != null) { return; }
            lock (LockObject)
            {
                if (SaveSqlRecord.ContainsKey(typeof(TEntity)) && SaveSqlRecord[typeof(TEntity)] != null) { return; }

                // 创建延迟执行
                var timer = new Timer(o =>
                {
                    lock (LockObject)
                    {
                        // 读取本地日志文件
                        var lst = Serialize.Load<List<TEntity>>(_filePath, _fileName, true) ?? new List<TEntity>();
                        var count = SqlLogList.Count;
                        lst.AddRange(SqlLogList);
                        SqlLogList.RemoveRange(0, count);
                        Serialize.Save(lst, _filePath, _fileName);

                        if (SqlLogList.Count == 0 && SaveSqlRecord.ContainsKey(typeof(TEntity)) && SaveSqlRecord[typeof(TEntity)] != null)
                        {
                            SaveSqlRecord[typeof(TEntity)].Dispose();
                            SaveSqlRecord[typeof(TEntity)] = null;
                            SaveSqlRecord.Remove(typeof(TEntity));
                        }
                    }
                }, null, 1000 * _lazyTime, 1000 * _lazyTime);
                SaveSqlRecord[typeof(TEntity)] = timer;
            }
        }

        protected virtual void SendEmail()
        {
            var mail = ExceptionEmailConfigs.ConfigEntity;
            var smtp = new SmtpMail(mail.LoginName, mail.LoginPwd, mail.SendMail, $"Farseer.Net {EnumNameCacheManger.Cache(logType)}记录", mail.SmtpServer, 0, mail.SmtpPort);
            var body = new StringBuilder();
            body.AppendFormat("<b>发现时间：</b> {0}<br />", CreateAt.ToString("yyyy年MM月dd日 HH:mm:ss"));
            body.AppendFormat("<b>程序文件：</b> <u>{0}</u> <b>第{1}行</b> <font color=red>{2}()</font><br />", FileName, LineNo, MethodName);
            body.AppendFormat("<b>日志消息：</b><font color=red>{0}</font><br />", Message);
            smtp.Send(mail.EmailAddress, $"{DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss")}：{EnumNameCacheManger.Cache(logType)}消息：{Message}", body.ToString());
        }

        public abstract void AddToQueue();
    }
}