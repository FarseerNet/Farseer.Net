using System.Collections.Generic;
using System.Reflection;
using FS.Configuration;
using FS.DI;
using FS.Modules;
using FS.MQ.RabbitMQ.Configuration;

namespace FS.MQ.RabbitMQ
{
    /// <summary>
    ///     RocketMQ模块
    /// </summary>
    public class RabbitModule : FarseerModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            // 如果Redis配置没有创建，则创建它
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        /// <inheritdoc />
        private void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.RabbitConfig();
            if (config == null || config.Items.Count == 0)
            {
                configResolver.Set(new RabbitConfig
                {
                    Items = new List<RabbitItemConfig>
                    {
                        new RabbitItemConfig
                        {
                            Server = "",
                            UserName = "",
                            Password = "",
                            Port = 5672,
                            Product = new List<ProductConfig>() {new ProductConfig() {Name = "test1", ExchangeName = "", RoutingKey = "", UseConfirmModel = false}},
                            Consumer = new List<ConsumerConfig>() {new ConsumerConfig() {Name = "test2", QueueName = ""}},
                        }
                    }
                });
                configResolver.Save();
            }
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new RabbitInstaller());
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
        }
    }
}