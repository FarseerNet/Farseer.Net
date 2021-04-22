using FS.Configuration;
using FS.Data;
using FS.Data.Internal;
using FS.Modules;

namespace Farseer.Net.Data.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(DataModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}
