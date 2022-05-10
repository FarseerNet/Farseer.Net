using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var sw         = Stopwatch.StartNew();
            var iocManager = container.Resolve<IIocManager>();

            // 读取配置
            var queueConfigs = QueueRoot.Get().ToList();

            // 从消费中找到未包含配置文件的。并生成到配置中 耗时：31 ms
            var consumerAttributes = _typeFinder.Find<IListenerMessage>().Select(o => o.GetCustomAttribute<ConsumerAttribute>()).Where(o => o != null);
            foreach (var consumerAtt in consumerAttributes)
            {
                if (!queueConfigs.Exists(o => o.Name == consumerAtt.Name))
                    queueConfigs.Add(new QueueConfig
                    {
                        Name      = consumerAtt.Name,
                        PullCount = consumerAtt.PullCount,
                        MaxCount  = consumerAtt.MaxCount,
                        SleepTime = consumerAtt.SleepTime
                    });
            }
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
            var queueAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<QueueAttribute>();
            if (queueAttribute is
                {
                    Enable: false
                }) return;


            Dictionary<string, Type> dicConsumerName = new();
            // 启动单次消费程序
            foreach (var consumerType in _typeFinder.Find<IListenerMessage>())
            {
                var consumerAtt = consumerType.GetCustomAttribute<ConsumerAttribute>();
                if (consumerAtt is
                    {
                        Enable: false
                    })
                    continue;

                var queueConfig = queueConfigs.FirstOrDefault(o => o.Name == consumerAtt.Name);
                // 重复队列名，检查
                if (dicConsumerName.ContainsKey(consumerAtt.Name)) throw new Exception($"Farseer.Net.MQ.Queue组件发现，已经包含了同名的消费：{consumerAtt.Name} ===> {dicConsumerName[consumerAtt.Name].FullName}、{consumerType.FullName}，Queue组件只允许一个队列名称，对应一个消费");
                dicConsumerName.Add(consumerAtt.Name, consumerType);

                if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);

                FarseerApplication.AddInitCallback(act: () =>
                {
                    // 注册消费端
                    iocManager.Logger<QueueInstaller>().LogInformation(message: $"正在启动Queue消费：{consumerAtt.Name}");
                    var consumerInstance = new QueueConsumer(iocManager: iocManager, consumerType: consumerType, queueConfig: queueConfig);
                    consumerInstance.StartWhile();
                });
            }
        }
    }
}