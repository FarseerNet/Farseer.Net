using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Farseer.Net.Configuration;
using FS.Configuration;
using FS.MQ.Kafka.Configuration;

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
            var localConfigResolver = container.Resolve<IConfigResolver>();
            if (localConfigResolver.KafkaConfig().Items.Count == 0) { return; }

            //注册所有的消息队列的Topic消费者
            localConfigResolver.KafkaConfig().Items.ForEach(c => container.Register(Component.For<IKafkaManager>()
                                                                           .Named(c.Name)
                                                                           .ImplementedBy<KafkaManager>()
                                                                           .DependsOn(Dependency.OnValue<KafkaItemConfig>(c))
                                                                           .LifestyleSingleton()));
        }
    }
}
