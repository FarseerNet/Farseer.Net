using System.Reflection;
using FS.Modules;

namespace Farseer.Net.Demo
{
    /// <summary>
    ///     启动模块
    /// </summary>
    [DependsOn]
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            IocManager.RegisterAssemblyByConvention(this.GetType());
        }
    }
}