using FS.Configuration;
using FS.Modules;
using FS.MQ.Rocket;
using FS.MQ.RocketMQ;

namespace Farseer.Net.MQ.Rocket.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(RocketModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}
