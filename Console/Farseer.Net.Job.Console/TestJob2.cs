using System.Threading;
using System.Threading.Tasks;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Console
{
    public class TestJob2 : IJob
    {
        public string Name { get; } = "延迟执行";
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