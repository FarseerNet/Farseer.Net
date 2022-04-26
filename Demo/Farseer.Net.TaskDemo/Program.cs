using FS;
using FS.Tasks;

namespace Farseer.Net.TaskDemo
{
    [Tasks] // 开启后，才能把JOB自动注册进来
    public class Program
    {
        public static void Main()
        {
            // 初始化模块
            FarseerApplication.Run<StartupModule>().Initialize();
            Thread.Sleep(millisecondsTimeout: -1);
        }
    }
}