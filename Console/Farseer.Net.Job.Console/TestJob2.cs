using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob2 : IJob
    {
        public JobSetting Setting { get; }=new JobSetting(2,"TestJob2：延迟执行");

        public void Init()
        {
        }

        public void Start(CancellationToken token,bool isAsyncRun)
        {
            for (int i = 0; i < 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(i + 1);
            }

            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}