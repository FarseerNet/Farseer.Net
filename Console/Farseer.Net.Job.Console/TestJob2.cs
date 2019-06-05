using System.Threading;
using System.Threading.Tasks;
using FS.Job;

namespace Farseer.Net.Job.Console
{
    public class TestJob2 : IJob
    {
        public string Name { get; } = "延迟执行";

        public void Init()
        {
        }

        public void Start()
        {
            Thread.Sleep(5000);
            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}