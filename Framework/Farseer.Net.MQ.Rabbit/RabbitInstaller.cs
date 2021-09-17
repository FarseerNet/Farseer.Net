using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.MQ.Rabbit.Attr;
using FS.MQ.Rabbit.Configuration;
using FS.Reflection;
using Microsoft.Extensions.Logging;

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
            var rabbitItemConfigs = RabbitConfigRoot.Get();

            // 注册生产者
            foreach (var rabbitItemConfig in rabbitItemConfigs)
            {
                // 每个Item，建立一个tcp连接
                var rabbitConnect = new RabbitConnect(rabbitItemConfig);

                if (rabbitItemConfig.Product != null)
                {
                    // 按生产者遍历
                    foreach (var productConfig in rabbitItemConfig.Product)
                    {
                        // 注册生产者
                        container.Register(Component.For<IRabbitManager>().Named(productConfig.Name).ImplementedBy<RabbitManager>().DependsOn(Dependency.OnValue<RabbitConnect>(rabbitConnect), Dependency.OnValue<ProductConfig>(productConfig)).LifestyleSingleton());
                        // 自动创建交换器
                        if (productConfig.AutoCreateExchange) container.Resolve<IRabbitManager>(productConfig.Name).CreateExchange();
                    }
                }
            }

            // 查找入口方法是否启用了Rabbit消费
            var rabbitAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<RabbitAttribute>();
            if (rabbitAttribute is { Enable: true })
            {
                var iocManager = container.Resolve<IIocManager>();
                try
                {
                    // 启动单次消费程序
                    foreach (var consumerType in container.Resolve<IAssemblyFinder>().GetType<IListenerMessage>())
                    {
                        var consumerAtt  = consumerType.GetCustomAttribute<ConsumerAttribute>();
                        if (consumerAtt is { Enable: false }) continue;
                        
                        if (!iocManager.IsRegistered(consumerType.FullName)) iocManager.Register(consumerType, consumerType.FullName, DependencyLifeStyle.Transient);
                        
                        FarseerApplication.AddInitCallback(() =>
                        {
                            iocManager.Resolve<IListenerMessage>(consumerType.FullName).Init(iocManager, consumerAtt, consumerType);
                        });
                        
                    }

                    // 启动批量消费程序
                    foreach (var consumerType in container.Resolve<IAssemblyFinder>().GetType<IListenerMessageBatch>())
                    {
                        var consumerAtt = consumerType.GetCustomAttribute<ConsumerAttribute>();
                        if (consumerAtt is { Enable: false }) continue;
                        
                        if (!iocManager.IsRegistered(consumerType.FullName)) iocManager.Register(consumerType, consumerType.FullName, DependencyLifeStyle.Transient);

                        FarseerApplication.AddInitCallback(() =>
                        {
                            Task.WaitAll(iocManager.Resolve<IListenerMessageBatch>(consumerType.FullName).Init(iocManager, consumerAtt, consumerType));
                        });
                        
                    }

                    IocManager.Instance.Logger<RabbitInstaller>().LogInformation("全部消费启动完成!");
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<RabbitInstaller>().LogError(e, e.Message);
                }
            }
        }
    }
}