using System.Threading;
using FS;
using FS.Job;

namespace Farseer.Net.Job.Console
{
    [Fss] // 开启后，才能注册到FSS平台
    public class Program
    {
        public static void Main()
        {
            // 初始化模块
            FarseerApplication.Run<StartupModule>().Initialize();
            Thread.Sleep(-1);
        }
    }
}