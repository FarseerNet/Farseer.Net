using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.EventBus.Attr;
using FS.EventBus.Configuration;
using FS.Reflection;
using Microsoft.Extensions.Logging;

namespace FS.EventBus
{
    /// <summary>
    ///     RocketMQ的IOC注册
    /// </summary>
    public class EventBusInstaller : IWindsorInstaller
    {

        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // // 读取配置
            // var eventBusConfig = EventBusRoot.Get();
            //
            // // 注册生产者
            // foreach (var eventConfig in eventBusConfig.Events)
            // {
            //     // 注册生产者
            //     container.Register(Component.For<IEventProduct>().Named(name: eventConfig.EventName).ImplementedBy<EventProduct>().DependsOn(Dependency.OnValue<string>(eventConfig.EventName)).LifestyleSingleton());
            // }

            var iocManager = container.Resolve<IIocManager>();
            try
            {
                // 启动订阅程序
                foreach (var consumerType in container.Resolve<IAssemblyFinder>().GetType<IListenerMessage>())
                {
                    var consumerAtt = consumerType.GetCustomAttribute<ConsumerAttribute>();

                    // 注册生产者
                    if (!container.Kernel.HasComponent(consumerAtt.EventName))
                    {
                        container.Register(Component.For<IEventProduct>().Named(consumerAtt.EventName).ImplementedBy<EventProduct>().DependsOn(Dependency.OnValue<string>(consumerAtt.EventName)).LifestyleSingleton());
                    }

                    if (consumerAtt is { Enable: false }) continue;

                    // 注册
                    if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);
                    
                    // 订阅事件
                    container.Resolve<IEventProduct>(consumerAtt.EventName).Subscribe(consumerType.FullName);
                }

                IocManager.Instance.Logger<EventBusInstaller>().LogInformation(message: "全部事件总线订阅启动完成!");
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<EventBusInstaller>().LogError(exception: e, message: e.Message);
            }
        }
    }
}