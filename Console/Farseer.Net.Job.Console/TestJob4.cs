using System;
using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob4 : IJob
    {
        public JobSetting Setting { get; }=new JobSetting(4,"TestJob4：测试ing?");

        public void Init()
        {
        }

        public void Start(CancellationToken token,bool isAsyncRun)
        {
            Nullable<int> count1 = null;
            int count2 = 2;
            System.Console.WriteLine(count1 < count2);
            System.Console.WriteLine(count1.GetValueOrDefault() < count2);
        }

        public void Stop()
        {
        }
    }
}