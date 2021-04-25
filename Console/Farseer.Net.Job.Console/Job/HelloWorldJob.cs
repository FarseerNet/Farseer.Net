using FS.DI;
using FS.Job;
using FS.Job.Attr;
using Microsoft.Extensions.Logging;

namespace Farseer.Net.Job.Console.Job
{
    [FssJob(Name = "bbb")] // Name与FSS平台配置的JobTypeName保持一致
    public class HelloWorldJob : IFssJob
    {
        public bool Invoke()
        {
            IocManager.Instance.Logger<HelloWorldJob>().LogInformation("你好，世界！");
            return true;
        }
    }
}