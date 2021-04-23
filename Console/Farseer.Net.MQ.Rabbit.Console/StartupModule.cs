using FS.Configuration;
using FS.Modules;
using FS.MQ.Rabbit;

namespace Farseer.Net.MQ.Rabbit.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(RabbitModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
        }
    }
}