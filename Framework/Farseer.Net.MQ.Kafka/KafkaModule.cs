using System.Reflection;
using FS.DI;
using FS.Modules;

namespace FS.MQ.Kafka
{
    /// <summary>
    ///     Kafka 模块
    /// </summary>
    public class KafkaModule : FarseerModule
    {
        /// <summary>
        ///     模块初始化前
        /// </summary>
        public override void PreInitialize()
        {
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new KafkaInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}