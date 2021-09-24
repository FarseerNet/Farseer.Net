using System.Reflection;
using FS.DI;
using FS.Modules;

namespace Farseer.Net.Grpc
{
    /// <summary>
    ///     Grpc模块
    /// </summary>
    public class GrpcModule : FarseerModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new GrpcInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}