using System;
using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob3 : IJob
    {
        public string Name { get; } = "TestJob3：异常执行";
        public JobSetting Setting { get; }

        public void Init()
        {
        }

        public void Start(CancellationToken token)
        {
            throw new Exception("出错啦~~");
        }

        public void Stop()
        {
        }
    }
}