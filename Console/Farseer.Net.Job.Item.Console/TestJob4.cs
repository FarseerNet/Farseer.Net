using System;
using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob4 : IJob
    {
        public string Name { get; } = "间隔执行";
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