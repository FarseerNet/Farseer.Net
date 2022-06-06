using System.Reflection;
using FS.DI;
using FS.Modules;

namespace FS.Core
{
    /// <summary>
    ///     系统核心模块
    /// </summary>
    public sealed class FarseerCoreModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}