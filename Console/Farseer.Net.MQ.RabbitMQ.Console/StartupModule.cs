using FS.Configuration;
using FS.Modules;
using FS.MQ.RabbitMQ;

namespace Farseer.Net.MQ.RabbitMQ.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(ConfigurationModule),typeof(RabbitModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}
