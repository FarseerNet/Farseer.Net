using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     RocketMQ的IOC注册
    /// </summary>
    public class RabbitInstaller : IWindsorInstaller
    {
        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {            
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("Kafka");
            var rabbitItemConfigs    = configurationSection.GetChildren().Select(o => o.Get<RabbitItemConfig>()).ToList();

            //注册所有的消息队列的Topic消费者
            foreach (var rabbitItemConfig in rabbitItemConfigs)
            {
                // 每个Item，建立一个tcp连接
                var rabbitConnect = new RabbitConnect(rabbitItemConfig);
                
                if (rabbitItemConfig.Product != null)
                {
                    // 按生产者遍历
                    foreach (var productConfig in rabbitItemConfig.Product)
                    {
                        container.Register(Component.For<IRabbitManager>().Named(productConfig.Name).ImplementedBy<RabbitManager>().DependsOn(Dependency.OnValue<RabbitConnect>(rabbitConnect), Dependency.OnValue<ProductConfig>(productConfig)).LifestyleSingleton());
                    }
                }
            }
        }
    }
}