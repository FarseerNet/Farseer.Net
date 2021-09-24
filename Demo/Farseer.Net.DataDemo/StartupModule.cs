using FS.Data;
using FS.Modules;

namespace Farseer.Net.DataDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(DataModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}