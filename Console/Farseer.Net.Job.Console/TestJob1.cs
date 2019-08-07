using System.Threading;
using System.Threading.Tasks;
using FS.Job;
using FS.Job.Entity;

namespace Farseer.Net.Job.Console
{
    public class TestJob1 : IJob
    {
        public JobSetting Setting { get; } = new JobSetting(1, "TestJob1：普通执行");

        public void Init()
        {
        }

        public void Start(CancellationToken token,bool isAsyncRun)
        {
            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}