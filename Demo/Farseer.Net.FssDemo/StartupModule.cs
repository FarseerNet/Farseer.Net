using FS.Fss;
using FS.Modules;

namespace Farseer.Net.FssDemo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn(typeof(FssModule))] // 依赖Job模块
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}