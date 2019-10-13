using System;
using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob3 : IJob
    {
        public JobSetting Setting { get; }=new JobSetting(3,"TestJob3：异常执行");

        public void Init()
        {
        }

        public void Start(CancellationToken token,bool isAsyncRun)
        {
            throw new Exception("出错啦~~");
        }

        public void Stop()
        {
        }
    }
}