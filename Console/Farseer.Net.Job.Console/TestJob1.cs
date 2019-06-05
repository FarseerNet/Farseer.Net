using System.Threading.Tasks;
using FS.Job;

namespace Farseer.Net.Job.Console
{
    public class TestJob1 : IJob
    {
        public string Name { get; } = "测试普通执行";

        public void Init()
        {
        }

        public void Start()
        {
            System.Console.WriteLine($"Test~~~");
        }

        public void Stop()
        {
        }
    }
}