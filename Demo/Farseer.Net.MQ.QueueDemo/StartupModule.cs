using FS.Modules;
using FS.MQ.Queue;

namespace Farseer.Net.MQ.QueueDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(QueueModule))]
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