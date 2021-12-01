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
using Microsoft.Extensions.Logging;

namespace FS.MQ.RedisStream
{
    /// <summary>
    ///     RocketMQ的IOC注册
    /// </summary>
    public class RedisStreamInstaller : IWindsorInstaller
    {
        private readonly Dictionary<string, RedisCacheManager> dicRedisCacheManager = new();

        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var redisStreamConfigs = RedisStreamConfigRoot.Get();

            // 注册生产者
            foreach (var redisStreamConfig in redisStreamConfigs)
            {
                // 注册服务端
                var redisCacheManager = dicRedisCacheManager[redisStreamConfig.Server.Name] = new RedisCacheManager(redisStreamConfig.Server);

                if (redisStreamConfig.Product != null)
                {
                    // 按生产者遍历
                    foreach (var productConfig in redisStreamConfig.Product)
                    {
                        if (container.Kernel.HasComponent(productConfig.QueueName)) throw new FarseerException($"队列名称重复，注册RedisStream失败：Server={redisStreamConfig.Server.Name}，QueueName={productConfig.QueueName}");
                        // 注册生产者
                        container.Register(Component.For<IRedisStreamProduct>()
                                                    .ImplementedBy<RedisStreamProduct>()
                                                    .Named(name: productConfig.QueueName)
                                                    .DependsOn(Dependency.OnValue<IRedisCacheManager>(value: redisCacheManager),
                                                               Dependency.OnValue<ProductItemConfig>(value: productConfig))
                                                    .LifestyleSingleton());
                    }
                }
            }

            // 查找入口方法是否启用了消费
            var rabbitAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<RedisStreamAttribute>();
            if (rabbitAttribute is { Enable: false }) return;
            
            // 查找消费实现
            var types = container.Resolve<IAssemblyFinder>().GetType<IListenerMessage>();
            try
            {
                foreach (var consumerType in types)
                // 启动消费程序
                    RunConsumer(container: container, consumerType: consumerType, rabbitItemConfigs: redisStreamConfigs);
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RedisStreamInstaller>().LogError(exception: e, message: e.Message);
            }
        }

        /// <summary>
        ///     启动消费程序
        /// </summary>
        private void RunConsumer(IWindsorContainer container, Type consumerType, List<RedisStreamConfig> rabbitItemConfigs)
        {
            // 没有使用consumerAttribute特性的，不启用
            var consumerAttribute = consumerType.GetCustomAttribute<ConsumerAttribute>();
            if (consumerAttribute is { Enable: false }) return;

            // 创建消费的实例
            if (!dicRedisCacheManager.ContainsKey(consumerAttribute.Server))
            {
                IocManager.Instance.Logger<RedisStreamInstaller>().LogWarning(message: $"未找到：{consumerType.FullName}的 Redis 消费配置项：{consumerAttribute.Server}");
                return;
            }
            var redisCacheManager = dicRedisCacheManager[consumerAttribute.Server];

            // 自动创建消费组
            if (consumerAttribute.AutoCreateGroup)
            {
                // 先判断队列是否存在，不存在则创建
                var isExistsQueue = redisCacheManager.Db.KeyExists(key: consumerAttribute.QueueName);
                if (!isExistsQueue) redisCacheManager.Db.StreamAdd(key: consumerAttribute.QueueName, streamField: "data", streamValue: "init create queue");

                if (!string.IsNullOrWhiteSpace(value: consumerAttribute.GroupName))
                {
                    var streamGroupInfos = redisCacheManager.Db.StreamGroupInfo(key: consumerAttribute.QueueName);
                    var existsGroup      = streamGroupInfos.Any(predicate: o => o.Name == consumerAttribute.GroupName);
                    if (!existsGroup) redisCacheManager.Db.StreamCreateConsumerGroup(key: consumerAttribute.QueueName, groupName: consumerAttribute.GroupName);
                }
            }

            var iocManager       = container.Resolve<IIocManager>();
            var consumerInstance = new RedisStreamConsumer(iocManager: iocManager, consumerType: consumerType.FullName, redisCacheManager: redisCacheManager, queueName: consumerAttribute.QueueName, lastAckTimeoutRestart: consumerAttribute.LastAckTimeoutRestart, consumeThreadNums: consumerAttribute.ConsumeThreadNums, groupName: consumerAttribute.GroupName, pullCount: consumerAttribute.PullCount);

            // 注册消费端
            container.Register(Component.For<IListenerMessage>().ImplementedBy(type: consumerType).Named(name: consumerType.FullName).LifestyleTransient());

            FarseerApplication.AddInitCallback(act: () =>
            {
                IocManager.Instance.Logger<RedisStreamInstaller>().LogInformation(message: $"正在启动：{consumerType.Name} Redis消费");
                consumerInstance.Start();
            });
        }
    }
}