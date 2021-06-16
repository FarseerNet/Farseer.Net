using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.MQ.Rabbit.Attr;
using FS.MQ.Rabbit.Configuration;
using FS.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Activator = System.Activator;

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
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("Rabbit");
            var rabbitItemConfigs    = configurationSection.GetChildren().Select(o => o.Get<RabbitItemConfig>()).ToList();

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
            if (rabbitAttribute is {Enable: true})
            {
                // 查找消费实现
                var types = container.Resolve<IAssemblyFinder>().GetType<IListenerMessage>();

                try
                {
                    foreach (var consumerType in types)
                    {
                        // 启动消费程序
                        RunConsumer(container, consumerType, rabbitItemConfigs);
                    }

                    IocManager.Instance.Logger<RabbitInstaller>().LogInformation("全部消费启动完成!");
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<RabbitInstaller>().LogError(e, e.Message);
                }
            }
        }

        /// <summary>
        /// 启动消费程序
        /// </summary>
        /// <param name="consumerType"></param>
        /// <param name="rabbitItemConfigs"> </param>
        private static void RunConsumer(IWindsorContainer container, Type consumerType, List<RabbitItemConfig> rabbitItemConfigs)
        {
            // 没有使用consumerAttribute特性的，不启用
            var consumerAttribute = consumerType.GetCustomAttribute<ConsumerAttribute>();
            if (consumerAttribute is {Enable: false}) return;

            // 创建消费的实例
            var rabbitItemConfig = rabbitItemConfigs.Find(o => o.Name == consumerAttribute.Name);
            if (rabbitItemConfig == null)
            {
                IocManager.Instance.Logger<RabbitInstaller>().LogWarning($"未找到：{consumerType.FullName}的配置项：{consumerAttribute.Name}");
                return;
            }

            var iocManager       = container.Resolve<IIocManager>();
            var rabbitConnect    = new RabbitConnect(rabbitItemConfig);
            var consumerInstance = new RabbitConsumer(iocManager, consumerType.FullName, rabbitConnect, consumerAttribute.QueueName, consumerAttribute.LastAckTimeoutRestart, consumerAttribute.ConsumeThreadNums);

            // 启用启动绑定时，要创建交换器、队列，并绑定
            if (consumerAttribute.AutoCreateAndBind)
            {
                IocManager.Instance.Logger<RabbitInstaller>().LogInformation($"正在初始化：{consumerType.Name}");
                var rabbitManager = new RabbitManager(rabbitConnect);

                // 配置死信参数
                var arguments                                                                                           = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(consumerAttribute.DlxExchangeName)) arguments["x-dead-letter-exchange"]  = consumerAttribute.DlxExchangeName;
                if (!string.IsNullOrWhiteSpace(consumerAttribute.DlxRoutingKey)) arguments["x-dead-letter-routing-key"] = consumerAttribute.DlxRoutingKey;
                if (consumerAttribute.DlxTime > 0) arguments["x-message-ttl"]                                           = consumerAttribute.DlxTime;
                rabbitManager.CreateExchange(consumerAttribute.ExchangeName, consumerAttribute.ExchangeType);
                rabbitManager.CreateQueueAndBind(consumerAttribute.QueueName, consumerAttribute.ExchangeName, consumerAttribute.RoutingKey, arguments: arguments);
            }

            container.Register(Component.For<IListenerMessage>().ImplementedBy(consumerType).Named(consumerType.FullName).LifestyleTransient());

            // 注册消费端            
            FarseerApplication.AddInitCallback(() =>
            {
                IocManager.Instance.Logger<RabbitInstaller>().LogInformation($"正在启动：{consumerType.Name} Rabbit消费");
                consumerInstance.Start();
            });
        }
    }
}