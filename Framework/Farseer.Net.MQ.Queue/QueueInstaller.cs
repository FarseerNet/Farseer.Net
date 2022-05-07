using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.MQ.Queue.Attr;
using FS.MQ.Queue.Configuration;
using FS.Reflection;
using Microsoft.Extensions.Logging;

namespace FS.MQ.Queue
{
    /// <summary>
    ///     Queue的IOC注册
    /// </summary>
    public class QueueInstaller : IWindsorInstaller
    {
        public QueueInstaller(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        private readonly ITypeFinder _typeFinder;

        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var queueConfigs = QueueRoot.Get();

            // 注册生产者
            foreach (var queueConfig in queueConfigs)
            {
                // 注册队列数据
                container.Register(Component.For<IQueueList>()
                                            .Named(name: $"queue_list_{queueConfig.Name}")
                                            .ImplementedBy<QueueList>()
                                            .DependsOn(Dependency.OnValue<QueueConfig>(value: queueConfig)).LifestyleSingleton());
                
                // 注册生产者
                container.Register(Component.For<IQueueManager>()
                                            .Named(name: queueConfig.Name)
                                            .ImplementedBy<QueueManager>()
                                            .DependsOn(Dependency.OnValue<QueueConfig>(value: queueConfig)).LifestyleSingleton());
            }

            // 查找入口方法是否启用了Queue消费
            var rabbitAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<QueueAttribute>();
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

                    var queueConfig = queueConfigs.FirstOrDefault(o=>o.Name == consumerAtt.Name);
                    if (queueConfig == null) throw new FarseerException($"注册{consumerType.FullName}的Queue消费失败，在配置中找不到队列名称：{consumerAtt.Name}");
                    if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);
                    
                    FarseerApplication.AddInitCallback(act: () =>
                    {
                        // 注册消费端            
                        iocManager.Logger<QueueInstaller>().LogInformation(message: $"正在启动：{consumerAtt.Name} Queue消费");
                        var consumerInstance = new QueueConsumer(iocManager: iocManager, consumerType: consumerType, queueConfig: queueConfig);
                        consumerInstance.StartWhile();
                    });
                }

                IocManager.Instance.Logger<QueueInstaller>().LogInformation(message: "全部消费启动完成!");
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<QueueInstaller>().LogError(exception: e, message: e.Message);
            }
        }
    }
}