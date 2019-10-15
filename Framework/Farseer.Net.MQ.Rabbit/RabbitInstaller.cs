using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.MQ.RabbitMQ.Configuration;

namespace FS.MQ.RabbitMQ
{
    /// <summary>
    ///     RocketMQ的IOC注册
    /// </summary>
    public class RabbitInstaller : IWindsorInstaller
    {
        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var localConfigResolver = container.Resolve<IConfigResolver>();
            if (localConfigResolver.RabbitConfig().Items.Count == 0) return;

            //注册所有的消息队列的Topic消费者
            //localConfigResolver.RabbitConfig().Items.ForEach(c =>container.Register(Component.For<IRabbitManager>().Named(c.Name).ImplementedBy<RabbitManager>().DependsOn(Dependency.OnValue<RabbitItemConfig>(c)).LifestyleSingleton()));
            // 按服务器地址遍历
            foreach (var rabbitItemConfig in localConfigResolver.RabbitConfig().Items)
            {
                if (rabbitItemConfig.Product != null)
                {
                    // 按生产者遍历
                    foreach (var productConfig in rabbitItemConfig.Product)
                    {
                        container.Register(Component.For<IRabbitManager>().Named(productConfig.Name).ImplementedBy<RabbitManager>().DependsOn(Dependency.OnValue<RabbitServerConfig>(rabbitItemConfig), Dependency.OnValue<ProductConfig>(productConfig)).LifestyleSingleton());
                    }
                }

                if (rabbitItemConfig.Consumer != null)
                {
                    // 按生产者遍历
                    foreach (var consumerConfig in rabbitItemConfig.Consumer)
                    {
                        container.Register(Component.For<IRabbitManager>().Named(consumerConfig.Name).ImplementedBy<RabbitManager>().DependsOn(Dependency.OnValue<RabbitServerConfig>(rabbitItemConfig), Dependency.OnValue<ConsumerConfig>(consumerConfig)).LifestyleSingleton());
                    }
                }
            }
        }
    }
}