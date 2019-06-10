using System.Threading;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Item.Console
{
    public class TestJob3 : IJob
    {
        public string Name { get; } = "间隔执行";
        public JobSetting Setting { get; }

        public void Init()
        {
        }

        public void Start(CancellationToken token)
        {
            Thread.Sleep(5000);
            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}