using System.Collections.Generic;
using System.Reflection;
using Farseer.Net.Configuration;
using FS.Configuration;
using FS.DI;
using FS.Modules;
using FS.MQ.Kafka.Configuration;

namespace FS.MQ.Kafka
{
    /// <summary>
    /// Kafka 模块
    /// </summary>
    public class KafkaModule : FarseerModule
    {
        /// <summary>
        /// 模块初始化前
        /// </summary>
        public override void PreInitialize()
        {
            // 如果Redis配置没有创建，则创建它
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        private void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.KafkaConfig();
            if (config == null || config.Items.Count == 0)
            {
                configResolver.Set(new KafkaConfig { Items = new List<KafkaItemConfig> { new KafkaItemConfig { Name = "test", Server = "IP:Host" } } });
                configResolver.Save();
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new KafkaInstaller());
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}
