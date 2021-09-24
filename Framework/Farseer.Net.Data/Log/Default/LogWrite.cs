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
using FS.Data.Log.Default.Entity;

namespace FS.Data.Log.Default
{
    /// <summary>
    ///     日志持久化工具
    /// </summary>
    internal class LogWrite
    {
        /// <summary> 写入路径 </summary>
        private readonly string _filePath;

        /// <summary> 延迟写入时间 </summary>
        private readonly int _lazyTime;

        /// <summary> 日志类型 </summary>
        private readonly EumLogType _logType;

        /// <summary> SQL执行记录列表 </summary>
        private readonly List<object> _sqlLogList = new();

        /// <summary>
        ///     时间定时器
        /// </summary>
        private Timer _timerSave;

        /// <summary>
        ///     日志持久化工具
        /// </summary>
        /// <param name="logType"> 日志类型 </param>
        /// <param name="filePath"> 写入路径 </param>
        /// <param name="lazyTime"> 延迟写入时间 </param>
        internal LogWrite(EumLogType logType, string filePath, int lazyTime)
        {
            _logType  = logType;
            _lazyTime = lazyTime;
            _filePath = filePath;

            InitTimer();
        }

        /// <summary>
        ///     初始化定时器（持久化到日志文件的任务）
        /// </summary>
        private void InitTimer()
        {
            // 创建延迟执行
            _timerSave = new Timer(callback: o =>
            {
                // 取得待持久化的数量
                var count = _sqlLogList.Count;
                if (count == 0) return;
                try
                {
                    Directory.CreateDirectory(path: _filePath);
                    // 文件名
                    var fileName = $"{DateTime.Now:yy-MM-dd}.log";
                    // JSON序例化后保存的变量
                    var json   = new string[count];
                    var jsoner = new DataContractJsonSerializer(type: _sqlLogList[index: 0].GetType());
                    for (var i = 0; i < count; i++)
                        using (var stream = new MemoryStream())
                        {
                            // 序列化到流
                            jsoner.WriteObject(stream: stream, graph: _sqlLogList[index: i]);
                            // 存入到json数组
                            json[i] = Encoding.UTF8.GetString(bytes: stream.ToArray());
                        }

                    // 写入到日志文件
                    File.AppendAllLines(path: _filePath + fileName, contents: json);
                    // 移除待持久化队列
                    _sqlLogList.RemoveRange(index: 0, count: count);
                }
                catch (Exception)
                {
                    // ignored
                }
            }, state: null, dueTime: 1000 * _lazyTime, period: 1000 * _lazyTime); // 指定延迟时间写入数据
        }

        /// <summary>
        ///     将对象添加到待持久化队列
        /// </summary>
        internal void Add(object obj) => _sqlLogList.Add(item: obj);

        /// <summary>
        ///     添加日志消息
        /// </summary>
        /// <param name="message"> 消息内容 </param>
        /// <param name="exp"> 异常 </param>
        public void Write(string message, Exception exp)
        {
            var log = new CommonLog { Exp = exp, Message = message ?? exp.Message.Replace(oldValue: "\r\n", newValue: "") };
            log.RecordExecuteMethod();
            //写入日志
            Add(obj: log);
        }
    }
}