using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Collections.Pooled;
using FS.DI;
using FS.MQ.Rabbit.Attr;
using FS.MQ.Rabbit.Configuration;
using FS.Reflection;
using Microsoft.Extensions.Logging;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     Rabbit的IOC注册
    /// </summary>
    public class RabbitInstaller : IWindsorInstaller
    {
        public RabbitInstaller(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }
        
        private readonly ITypeFinder _typeFinder;

        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            using var rabbitItemConfigs = RabbitConfigRoot.Get().ToPooledList();

            // 注册生产者
            foreach (var rabbitItemConfig in rabbitItemConfigs)
            {
                // 每个Item，建立一个tcp连接
                var rabbitConnect = new RabbitConnect(config: rabbitItemConfig.Server);

                if (rabbitItemConfig.Product != null)
                {
                    // 按生产者遍历
                    foreach (var productConfig in rabbitItemConfig.Product)
                    {
                        // 注册生产者
                        container.Register(Component.For<IRabbitManager>().Named(name: productConfig.Name).ImplementedBy<RabbitManager>().DependsOn(Dependency.OnValue<RabbitConnect>(value: rabbitConnect), Dependency.OnValue<ProductConfig>(value: productConfig)).LifestyleSingleton());
                        // 自动创建交换器
                        if (productConfig.AutoCreateExchange) container.Resolve<IRabbitManager>(key: productConfig.Name).CreateExchange();
                    }
                    rabbitItemConfig.Product.Dispose();
                }
            }

            // 查找入口方法是否启用了Rabbit消费
            var rabbitAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<RabbitAttribute>();
            if (rabbitAttribute is not
            {
                Enable: true
            }) return;
            
            var iocManager = container.Resolve<IIocManager>();
            try
            {
                // 启动单次消费程序
                foreach (var consumerType in _typeFinder.Find<IListenerMessage>())
                {
                    var consumerAtt = consumerType.GetCustomAttribute<ConsumerAttribute>();
                    if (consumerAtt is
                    {
                        Enable: false
                    })
                        continue;

                    if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);

                    FarseerApplication.AddInitCallback(act: () =>
                    {
                        iocManager.Logger<RabbitInstaller>().LogInformation(message: $"正在启动Rabbit消费：{consumerAtt.QueueName}");
                        Task.WaitAll(iocManager.Resolve<IListenerMessage>(name: consumerType.FullName).Init(iocManager: iocManager, consumerAtt: consumerAtt, consumerType: consumerType));
                    });
                }

                // 启动批量消费程序
                foreach (var consumerType in _typeFinder.Find<IListenerMessageBatch>())
                {
                    var consumerAtt = consumerType.GetCustomAttribute<ConsumerAttribute>();
                    if (consumerAtt is
                    {
                        Enable: false
                    })
                        continue;

                    if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);

                    FarseerApplication.AddInitCallback(act: () =>
                    {
                        iocManager.Logger<RabbitInstaller>().LogInformation(message: $"正在启动Rabbit批量消费：{consumerAtt.QueueName}");
                        Task.WhenAll(iocManager.Resolve<IListenerMessageBatch>(name: consumerType.FullName).Init(iocManager: iocManager, consumerAtt: consumerAtt, consumerType: consumerType));
                    });
                }
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RabbitInstaller>().LogError(exception: e, message: e.Message);
            }
        }
    }
}