using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.MQ.Kafka.Configuration;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.Kafka
{
    /// <summary>
    /// Kafka IOC初始化
    /// </summary>
    public class KafkaInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Kafka IOC初始化
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("Kafka");
            var kafkaItemConfigs     = configurationSection.GetChildren().Select(o => o.Get<KafkaItemConfig>()).ToList();

            //注册所有的消息队列的Topic消费者
            kafkaItemConfigs.ForEach(c => container.Register(Component.For<IKafkaManager>()
                                                                           .Named(c.Name)
                                                                           .ImplementedBy<KafkaManager>()
                                                                           .DependsOn(Dependency.OnValue<KafkaItemConfig>(c))
                                                                           .LifestyleSingleton()));
        }
    }
}
