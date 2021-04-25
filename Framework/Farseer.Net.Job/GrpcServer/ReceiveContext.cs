using System;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FSS.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FS.Job.GrpcServer
{
    /// <summary>
    /// 任务接收的上下文
    /// </summary>
    public class ReceiveContext
    {
        private readonly IServerStreamWriter<JobInvokeResponse> _responseStream;
        private readonly IIocManager                            _ioc;
        public           int                                    Progress { get; internal set; }
        public           long                                   NextAt   { get; private set; }

        /// <summary>
        /// FSS平台请求的内容
        /// </summary>
        public JobInvokeRequest Request { get; set; }

        public ReceiveContext(IIocManager ioc, JobInvokeRequest request, IServerStreamWriter<JobInvokeResponse> responseStream)
        {
            _ioc                 = ioc;
            this.Request         = request;
            this._responseStream = responseStream;
            NextAt               = request.NextAt;
        }

        /// <summary>
        /// 返回进度0-100
        /// </summary>
        public Task SetProgress(int rate)
        {
            if (rate is < 0 or > 100) throw new Exception("ReceiveContext.SetProgress的rate只能是0-100");
            Progress = rate;
            return WriteAsync();
        }

        /// <summary>
        /// 写入到FSS平台的日志
        /// </summary>
        public void Logger(LogLevel logLevel, string log)
        {
            _ioc.Logger<ReceiveContext>().Log(logLevel, log);
            _responseStream.WriteAsync(new JobInvokeResponse
            {
                TaskId   = Request.TaskId,
                NextAt   = NextAt,
                Progress = Progress,
                Status   = 1,
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
        public Task SetNextAt(DateTime dt)
        {
            NextAt = dt.ToTimestamps();
            return WriteAsync();
        }

        /// <summary>
        /// 成功后执行
        /// </summary>
        /// <returns></returns>
        internal Task Success()
        {
            return _responseStream.WriteAsync(new JobInvokeResponse
            {
                TaskId   = Request.TaskId,
                NextAt   = NextAt,
                Progress = 100,
                Status   = 4
            });
        }

        /// <summary>
        /// 执行失败
        /// </summary>
        /// <returns></returns>
        public Task Fail()
        {
            return _responseStream.WriteAsync(new JobInvokeResponse
            {
                TaskId   = Request.TaskId,
                NextAt   = NextAt,
                Progress = Progress,
                Status   = 3
            });
        }

        private Task WriteAsync()
        {
            return _responseStream.WriteAsync(new JobInvokeResponse
            {
                TaskId   = Request.TaskId,
                NextAt   = NextAt,
                Progress = Progress,
                Status   = 1
            });
        }
    }
}