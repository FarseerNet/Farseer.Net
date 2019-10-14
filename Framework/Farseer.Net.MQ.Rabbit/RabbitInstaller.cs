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
            localConfigResolver.RabbitConfig().Items.ForEach(c =>
                container.Register(Component.For<IRabbitManager>().Named(c.Name).ImplementedBy<RabbitManager>()
                    .DependsOn(Dependency.OnValue<RabbitItemConfig>(c)).LifestyleSingleton()));
        }
    }
}