using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FS.Utils.Component
{
    /// <summary>
    ///     测试效率的工具
    ///     用于做平均效率测试
    /// </summary>
    public class SpeedTestMultiple : IDisposable
    {
        /// <summary>
        ///     锁定
        /// </summary>
        private readonly object _objLock = new object();

        /// <summary>
        ///     保存测试的结果
        /// </summary>
        public List<SpeedResult> ListResult = new List<SpeedResult>();

        /// <summary>
        ///     保存测试的结果
        /// </summary>
        public SpeedResult Result => ListResult.Last();

        /// <summary>
        ///     使用完后，自动计算时间
        /// </summary>
        public void Dispose()
        {
            ListResult.Last(o => o.Timer.IsRunning).Timer.Stop();
        }

        /// <summary>
        ///     开始计数
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public SpeedResult Begin(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) { throw new Exception("必须设置keyName的值！"); }

            var result = Create(keyName);
            result.Timer.Start();
            return result;
        }

        /// <summary>
        ///     开始计数
        /// </summary>
        public SpeedResult Begin()
        {
            var result = new SpeedResult { KeyName = null, Timer = new Stopwatch() };
            result.Timer.Start();

            ListResult = new List<SpeedResult> { result };
            return result;
        }

        /// <summary>
        ///     停止工作
        /// </summary>
        public void Stop(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) { throw new Exception("必须设置keyName的值！"); }

            Create(keyName);
            ListResult.FirstOrDefault(o => o.KeyName == keyName)?.Timer.Stop();
        }

        /// <summary>
        ///     判断键位是否存在（不存在，自动创建）
        /// </summary>
        private SpeedResult Create(string keyName)
        {
            var result = ListResult.Find(o => o.KeyName == keyName);
            if (result != null) return result;
            lock (_objLock)
            {
                if (ListResult.Count(o => o.KeyName == keyName) == 0) { ListResult.Add((result = new SpeedResult { KeyName = keyName, Timer = new Stopwatch() })); }
            }
            return result;
        }

        /// <summary>
        ///     返回执行结果
        /// </summary>
        public class SpeedResult : IDisposable
        {
            /// <summary>
            ///     当前键码
            /// </summary>
            public string KeyName;

            /// <summary>
            ///     当前时间计数器
            /// </summary>
            public Stopwatch Timer;

            /// <summary>
            ///     停止工作
            /// </summary>
            public void Stop()
            {
                Timer.Stop();
            }

            /// <summary>
            ///     使用完后，自动计算时间
            /// </summary>
            public void Dispose()
            {
                Stop();
                Timer.Reset();
            }
        }
    }
}