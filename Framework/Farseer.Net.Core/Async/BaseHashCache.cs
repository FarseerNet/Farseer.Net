﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FS.Core.Async
{
    /// <summary>
    ///     本地进程内的hash数据缓存
    /// </summary>
    public abstract class BaseHashCache<T>
    {
        ///// <summary> 同步锁</summary>
        //static readonly object _locker = new object();

        /// <summary>
        ///     定时拉取数据的任务
        /// </summary>
        private Task _asnycTask;

        /// <summary>
        ///     缓存数据
        /// </summary>
        private Dictionary<string, T> Fields = new();

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="asyncInterval"> </param>
        public BaseHashCache(int asyncInterval = 30 * 1000)
        {
            AsyncInterval = asyncInterval;
        }

        /// <summary>
        ///     同步到本地的时间间隔
        /// </summary>
        public int AsyncInterval { get; set; }

        /// <summary>
        ///     获取所有缓存信息
        /// </summary>
        /// <returns> </returns>
        public Dictionary<string, T> GetAll() => Fields;

        public T Get(string fieldName)
        {
            var filedNameLower = fieldName.Trim().ToLower();
            if (Fields.ContainsKey(key: filedNameLower)) return Fields[key: filedNameLower];
            return default;
        }

        /// <summary>
        ///     有则修改,无则增加
        /// </summary>
        /// <param name="fieldName"> </param>
        /// <param name="value"> </param>
        public void Set(string fieldName, T value)
        {
            var filedNameLower = fieldName.Trim().ToLower();
            if (Fields.ContainsKey(key: filedNameLower))
                Fields[key: filedNameLower] = value;
            else
                Fields.Add(key: filedNameLower, value: value);
        }


        /// <summary>
        ///     初始化,立即执行数据同步,并开启线程,定时同步数据
        /// </summary>
        public void Init(CancellationToken cancellationToken)
        {
            AsyncFields();
            Async(cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     获取原始数据
        /// </summary>
        /// <returns> </returns>
        protected abstract Dictionary<string, T> GetSourceData();

        /// <summary>
        ///     同步数据
        /// </summary>
        /// <param name="cancellationToken"> 取消令牌 </param>
        private void Async(CancellationToken cancellationToken)
        {
            _asnycTask = new Task(action: () =>
            {
                LoopAsync(cancellationToken: cancellationToken);
            }, creationOptions: TaskCreationOptions.LongRunning);
            _asnycTask.Start();
        }

        /// <summary>
        ///     循环定时同步
        /// </summary>
        /// <param name="cancellationToken"> </param>
        private void LoopAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    AsyncFields();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(value: e);
                }

                Sleep(totalSleepMilliseconds: AsyncInterval, ct: cancellationToken);
            }
        }

        /// <summary>
        ///     同步数据,
        /// </summary>
        /// <returns> </returns>
        private void AsyncFields()
        {
            //拉取原始数据,替换当前的缓存
            Fields = GetSourceData();
        }


        /// <summary>
        ///     线程休眠,5ms检测一次取消令牌
        /// </summary>
        /// <param name="totalSleepMilliseconds"> </param>
        /// <param name="ct"> </param>
        private static void Sleep(long totalSleepMilliseconds, CancellationToken ct)
        {
            try
            {
                var leftSleep = totalSleepMilliseconds;
                var sw        = new Stopwatch();
                var start     = Environment.TickCount;
                while (true)
                {
                    ct.ThrowIfCancellationRequested(); //检测取消令牌,取消执行

                    if (leftSleep < 5)
                    {
                        if (leftSleep >= 0)
                            Thread.Sleep(millisecondsTimeout: (int)leftSleep);
                        else
                            Thread.Sleep(millisecondsTimeout: 1);
                        return;
                    }

                    Thread.Sleep(millisecondsTimeout: 5);

                    var stop = Environment.TickCount;
                    leftSleep = totalSleepMilliseconds - (stop - start);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(value: e);
            }
        }
    }
}