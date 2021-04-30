using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FSS.GrpcService;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FS.Job.Entity
{
    /// <summary>
    /// 任务接收的上下文
    /// </summary>
    public class ReceiveContext
    {
        private readonly Stopwatch                                                   _sw;
        private readonly IIocManager                                                 _ioc;
        private readonly AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> _rpc;
        internal         int                                                         Progress { get; set; }
        private          long                                                        _nextAt;
        private readonly Queue<UploadJobProgress>                                    _logQueue = new();
        private          TimeSpan?                                                   _ts;

        public ReceiveContext(IIocManager ioc, AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> rpc, Stopwatch sw)
        {
            _ioc = ioc;
            _rpc = rpc;
            _sw  = sw;
        }

        /// <summary>
        /// 返回进度0-100
        /// </summary>
        public void SetProgress(int rate)
        {
            if (rate is < 0 or > 100) throw new Exception("ReceiveContext.SetProgress的rate只能是0-100");
            Progress = rate;
        }

        /// <summary>
        /// 本次执行完后，下一次执行的间隔时间
        /// </summary>
        public void SetNextAt(TimeSpan ts)
        {
            _ts = ts;
        }

        /// <summary>
        /// 写入到FSS平台的日志
        /// </summary>
        public void Logger(LogLevel logLevel, string log)
        {
            _ioc.Logger<ReceiveContext>().Log(logLevel, log);
            _logQueue.Enqueue(new UploadJobProgress
            {
                NextAt   = _nextAt,
                Progress = Progress,
                RunSpeed = (int) _sw.ElapsedMilliseconds,
                Log = new LogResponse
                {
                    LogLevel = (int) logLevel,
                    Log      = log,
                    CreateAt = DateTime.Now.ToTimestamps()
                }
            });
        }

        /// <summary>
        /// 成功后执行
        /// </summary>
        internal async Task SuccessAsync(LogResponse log = null)
        {
            await UploadQueueAsync();

            // 如果本次有动态设计时间
            if (_ts.HasValue) _nextAt = (int)_ts.GetValueOrDefault().TotalMilliseconds;

            await _rpc.RequestStream.WriteAsync(new JobInvokeRequest
            {
                NextAt   = _nextAt,
                Progress = 100,
                Status   = 4,
                RunSpeed = (int) _sw.ElapsedMilliseconds,
                Log      = log
            });
        }

        /// <summary>
        /// 执行失败
        /// </summary>
        public async Task FailAsync(LogResponse log = null)
        {
            await UploadQueueAsync();

            // 如果本次有动态设计时间
            if (_ts.HasValue) _nextAt = (int)_ts.GetValueOrDefault().TotalMilliseconds;

            await _rpc.RequestStream.WriteAsync(new JobInvokeRequest
            {
                NextAt   = _nextAt,
                Progress = Progress,
                Status   = 3,
                RunSpeed = (int) _sw.ElapsedMilliseconds,
                Log      = log
            });
        }

        /// <summary>
        /// 上传队列中的数据
        /// </summary>
        internal async Task UploadAsync()
        {
            // 队列没有数据时，提交下当前进度信息
            if (_logQueue.Count == 0 && Progress > 0)
            {
                await _rpc.RequestStream.WriteAsync(new JobInvokeRequest
                {
                    NextAt   = _nextAt,
                    Progress = Progress,
                    Status   = 2,
                    RunSpeed = (int) _sw.ElapsedMilliseconds,
                });
            }
            else await UploadQueueAsync();
        }

        /// <summary>
        /// 上传队列中所有数据
        /// </summary>
        private async Task UploadQueueAsync()
        {
            while (_logQueue.Count > 0)
            {
                var log = _logQueue.Dequeue();
                await _rpc.RequestStream.WriteAsync(new JobInvokeRequest
                {
                    NextAt   = _nextAt,
                    Progress = log.Progress,
                    Status   = 2,
                    RunSpeed = log.RunSpeed,
                    Log      = log.Log
                });
            }
        }
    }
}