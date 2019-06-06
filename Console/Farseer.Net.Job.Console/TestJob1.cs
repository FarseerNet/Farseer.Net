using System.Threading;
using System.Threading.Tasks;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Console
{
    public class TestJob1 : IJob
    {
        public string Name { get; } = "测试普通执行";
        public JobSetting Setting { get; }

        public void Init()
        {
        }

        public void Start(CancellationToken token)
        {
            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}