using FS.EventBus;
using FS.Modules;

namespace Farseer.Net.EventBusDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(EventBusModule))]
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