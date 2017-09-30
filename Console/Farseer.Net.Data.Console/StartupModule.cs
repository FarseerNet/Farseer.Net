using Farseer.Net.Configuration;
using Farseer.Net.Modules;

namespace Farseer.Net.Data.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(ConfigurationModule),typeof(DataModule))]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}
