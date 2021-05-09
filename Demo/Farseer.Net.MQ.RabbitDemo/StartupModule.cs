using FS.Modules;
using FS.MQ.Rabbit;

namespace Farseer.Net.MQ.RabbitDemo
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