using System.Threading;
using FS;
using FS.Fss;

namespace Farseer.Net.FssDemo
{
    [Fss] // 开启后，才能注册到FSS平台
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