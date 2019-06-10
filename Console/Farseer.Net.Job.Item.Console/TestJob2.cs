using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob2 : IJob
    {
        public string Name { get; } = "TestJob2：延迟执行";
        public JobSetting Setting { get; }

        public void Init()
        {
        }

        public void Start(CancellationToken token)
        {
            for (int i = 0; i < 50000; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Thread.Sleep(i);
            }
            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}