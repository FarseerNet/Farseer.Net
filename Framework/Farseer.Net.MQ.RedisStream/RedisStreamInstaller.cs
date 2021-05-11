using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Cache.Redis;
using FS.DI;
using FS.MQ.RedisStream.Attr;
using FS.MQ.RedisStream.Configuration;
using FS.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Activator = System.Activator;

namespace FS.MQ.RedisStream
{
    /// <summary>
    ///     RocketMQ的IOC注册
    /// </summary>
    public class RedisStreamInstaller : IWindsorInstaller
    {
        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("RedisStream");
            var redisStreamConfigs   = configurationSection.GetChildren().Select(o => o.Get<RedisStreamConfig>()).ToList();

            // 注册生产者
            foreach (var redisStreamConfig in redisStreamConfigs)
            {
                if (redisStreamConfig.Product != null)
                {
                    // 按生产者遍历
                    foreach (var productConfig in redisStreamConfig.Product)
                    {
                        if (!container.Kernel.HasComponent(redisStreamConfig.RedisName))
                        {
                            IocManager.Instance.Logger<RedisStreamInstaller>().LogWarning($"未找到：{redisStreamConfig.RedisName}的 Redis 生产配置项，当前组件依赖Farseer.Net.Cache.Redis组件。因此您 需要先设置Redis的配置");
                            continue;
                        }

                        var redisCacheManager = container.Resolve<IRedisCacheManager>(redisStreamConfig.RedisName);

                        // 注册生产者
                        container.Register(Component.For<IRedisStreamProduct>()
                            .ImplementedBy<RedisStreamProduct>()
                            .Named(productConfig.QueueName)
                            .DependsOn(Dependency.OnValue<IRedisCacheManager>(redisCacheManager),
                                Dependency.OnValue<ProductItemConfig>(productConfig))
                            .LifestyleSingleton());
                    }
                }
            }

            // 查找入口方法是否启用了Rabbit消费
            var rabbitAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<RedisStreamAttribute>();
            if (rabbitAttribute is {Enable: true})
            {
                // 查找消费实现
                var types = container.Resolve<IAssemblyFinder>().GetType<IListenerMessage>();

                try
                {
                    foreach (var consumerType in types)
                    {
                        // 启动消费程序
                        RunConsumer(container, consumerType, redisStreamConfigs);
                    }

                    IocManager.Instance.Logger<RedisStreamInstaller>().LogInformation("全部消费启动完成!");
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<RedisStreamInstaller>().LogError(e, e.Message);
                }
            }
        }

        /// <summary>
        /// 启动消费程序
        /// </summary>
        /// <param name="consumerType"></param>
        /// <param name="rabbitItemConfigs"> </param>
        private static void RunConsumer(IWindsorContainer container, Type consumerType, List<RedisStreamConfig> rabbitItemConfigs)
        {
            // 没有使用consumerAttribute特性的，不启用
            var consumerAttribute = consumerType.GetCustomAttribute<ConsumerAttribute>();
            if (consumerAttribute is {Enable: false}) return;

            // 创建消费的实例
            var redisStreamConfig = rabbitItemConfigs.Find(o => o.RedisName == consumerAttribute.RedisName);
            if (redisStreamConfig == null)
            {
                IocManager.Instance.Logger<RedisStreamInstaller>().LogWarning($"未找到：{consumerType.FullName}的 Redis 消费配置项：{consumerAttribute.RedisName}");
                return;
            }

            if (!container.Kernel.HasComponent(redisStreamConfig.RedisName))
            {
                IocManager.Instance.Logger<RedisStreamInstaller>().LogWarning($"未找到：{consumerType.FullName}的 Redis 消费配置项：{consumerAttribute.RedisName}，当前组件依赖Farseer.Net.Cache.Redis组件。因此您 需要先设置Redis的配置");
                return;
            }

            var redisCacheManager = container.Resolve<IRedisCacheManager>(redisStreamConfig.RedisName);

            // 自动创建消费组
            if (consumerAttribute.AutoCreateGroup)
            {
                // 先判断队列是否存在，不存在则创建
                var isExistsQueue = redisCacheManager.Db.KeyExists(consumerAttribute.QueueName);
                if (!isExistsQueue) redisCacheManager.Db.StreamAdd(consumerAttribute.QueueName, "data", "init create queue");
                
                var streamGroupInfos = redisCacheManager.Db.StreamGroupInfo(consumerAttribute.QueueName);
                var existsGroup      = streamGroupInfos.Any(o => o.Name == consumerAttribute.GroupName);
                if (!existsGroup)
                    redisCacheManager.Db.StreamCreateConsumerGroup(consumerAttribute.QueueName, consumerAttribute.GroupName);
            }

            var iocManager       = container.Resolve<IIocManager>();
            var consumerInstance = new RedisStreamConsumer(iocManager, consumerType.FullName, redisCacheManager, consumerAttribute.QueueName, consumerAttribute.LastAckTimeoutRestart, consumerAttribute.ConsumeThreadNums, consumerAttribute.GroupName, consumerAttribute.PullCount);

            IocManager.Instance.Logger<RedisStreamInstaller>().LogInformation($"正在启动：{consumerType.Name}");

            // 注册消费端
            container.Register(Component.For<IListenerMessage>().ImplementedBy(consumerType).Named(consumerType.FullName).LifestyleTransient());
            consumerInstance.Start();
        }
    }
}