﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Cache.Redis.Configuration;
using FS.DI;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis依赖注册
    /// </summary>
    public class RedisInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public RedisInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     注册依赖
        /// </summary>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var redisItemConfigs = RedisConfigRoot.Get();

            foreach (var redisItemConfig in redisItemConfigs)
            {
                // 注册Redis连接
                // var redisConnectionWrapper = new RedisConnectionWrapper(redisItemConfig);
                //container.Register(Component.For<IRedisConnectionWrapper>().Named(name: $"{redisItemConfig.Name}_connection").Instance(redisConnectionWrapper).LifestyleSingleton());

                // 注册 GetCacheInRedis缓存到IGetCache
                container.Register(Component.For<ICacheManager>().ImplementedBy<CacheManager>().DependsOn(dependency: Dependency.OnValue<string>(value: redisItemConfig.Name)).Named(name: $"GetCacheInMemory_{redisItemConfig.Name}").LifestyleSingleton());
                container.Register(Component.For<IGetCache>().ImplementedBy<GetCacheInRedis>().DependsOn(dependency: Dependency.OnValue<string>(value: redisItemConfig.Name)).Named(name: $"GetCacheInRedis_{redisItemConfig.Name}").LifestyleSingleton());
                container.Register(Component.For<IGetCache>().ImplementedBy<GetCacheInMemoryAndRedis>().DependsOn(dependency: Dependency.OnValue<string>(value: redisItemConfig.Name)).Named(name: $"GetCacheInMemoryAndRedis_{redisItemConfig.Name}").LifestyleSingleton());

                // Redis缓存管理
                var redisCacheManager = new RedisCacheManager(config: redisItemConfig);

                // 注册Redis管理
                container.Register(Component.For<IRedisCacheManager>().Named(name: redisItemConfig.Name).Instance(instance: redisCacheManager).LifestyleSingleton());

                // 当配置项为"default"，或者只有一项时，注册一个不带别名的实例
                if (redisItemConfig.Name == "default" || redisItemConfigs.Count == 1)
                {
                    container.Register(Component.For<IRedisCacheManager>().Instance(instance: redisCacheManager).LifestyleSingleton());
                }
            }
        }
    }
}