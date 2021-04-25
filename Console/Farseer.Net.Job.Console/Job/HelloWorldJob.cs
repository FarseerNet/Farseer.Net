using FS.Job;
using FS.Job.Attr;
using FS.Job.GrpcServer;
using Microsoft.Extensions.Logging;

namespace Farseer.Net.Job.Console.Job
{
    [FssJob(Name = "bbb")] // Name与FSS平台配置的JobTypeName保持一致
    public class HelloWorldJob : IFssJob
    {
        public bool Execute(ReceiveContext context)
        {
            context.SetProgress(20);
            context.Logger(LogLevel.Information, "你好，世界！");
            context.SetProgress(100);
            return true;
        }
    }
}