using System;
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
        public           int                                                         Progress { get; internal set; }
        public           long                                                        NextAt   { get; private set; }

        public ReceiveContext(IIocManager ioc, AsyncClientStreamingCall<JobInvokeRequest, CommandResponse> rpc, long nextAt, Stopwatch sw)
        {
            _ioc   = ioc;
            _rpc   = rpc;
            _sw    = sw;
            NextAt = nextAt;
        }

        /// <summary>
        /// 返回进度0-100
        /// </summary>
        public Task SetProgressAsync(int rate)
        {
            if (rate is < 0 or > 100) throw new Exception("ReceiveContext.SetProgress的rate只能是0-100");
            Progress = rate;
            return WriteAsync();
        }

        /// <summary>
        /// 写入到FSS平台的日志
        /// </summary>
        public Task LoggerAsync(LogLevel logLevel, string log)
        {
            _ioc.Logger<ReceiveContext>().Log(logLevel, log);
            return _rpc.RequestStream.WriteAsync(new JobInvokeRequest
            {
                NextAt   = NextAt,
                Progress = Progress,
                Status   = 1,
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
        /// 设置下一次执行的时间
        /// </summary>
        public Task SetNextAtAsync(DateTime dt)
        {
            NextAt = dt.ToTimestamps();
            return WriteAsync();
        }

        /// <summary>
        /// 成功后执行
        /// </summary>
        /// <returns></returns>
        internal Task SuccessAsync(LogResponse log = null)
        {
            return _rpc.RequestStream.WriteAsync(new JobInvokeRequest
            {
                NextAt   = NextAt,
                Progress = 100,
                Status   = 3,
                RunSpeed = (int) _sw.ElapsedMilliseconds,
                Log      = log
            });
        }

        /// <summary>
        /// 执行失败
        /// </summary>
        /// <returns></returns>
        public Task FailAsync(LogResponse log = null)
        {
            return _rpc.RequestStream.WriteAsync(new JobInvokeRequest
            {
                NextAt   = NextAt,
                Progress = Progress,
                Status   = 2,
                RunSpeed = (int) _sw.ElapsedMilliseconds,
                Log      = log
            });
        }

        private Task WriteAsync(LogResponse log = null)
        {
            return _rpc.RequestStream.WriteAsync(new JobInvokeRequest
            {
                NextAt   = NextAt,
                Progress = Progress,
                Status   = 1,
                RunSpeed = (int) _sw.ElapsedMilliseconds,
                Log      = log
            });
        }
    }
}