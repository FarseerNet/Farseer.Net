using FS.Configuration;
using FS.Modules;
using FS.MQ.RocketMQ;

namespace Farseer.Net.MQ.RocketMQ.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(ConfigurationModule),typeof(RocketMQModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}
