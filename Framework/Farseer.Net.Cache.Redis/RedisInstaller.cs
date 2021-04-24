﻿using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Cache.Redis.Configuration;
using FS.Configuration;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Cache.Redis
{
    /// <summary>
    /// Redis依赖注册
    /// </summary>
    public class RedisInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public RedisInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("Redis");
            var redisItemConfigs     = configurationSection.GetChildren().Select(o => o.Get<RedisItemConfig>()).ToList();

            foreach (var redisItemConfig in redisItemConfigs)
            {
                // 注册Redis连接
                container.Register(Component.For<IRedisConnectionWrapper>().Named($"{redisItemConfig.Name}_connection").ImplementedBy<RedisConnectionWrapper>().DependsOn(Dependency.OnValue<RedisItemConfig>(redisItemConfig)).LifestyleSingleton());

                // 注册Redis管理
                container.Register(Component.For<IRedisCacheManager>().Named(redisItemConfig.Name).ImplementedBy<RedisCacheManager>().DependsOn(Dependency.OnValue<RedisItemConfig>(redisItemConfig), Dependency.OnValue<IRedisConnectionWrapper>(_iocResolver.Resolve<IRedisConnectionWrapper>($"{redisItemConfig.Name}_connection"))).LifestyleSingleton());

                // 当配置项为"default"，或者只有一项时，注册一个不带别名的实例
                if (redisItemConfig.Name == "default" || redisItemConfigs.Count == 1)
                {
                    container.Register(Component.For<IRedisCacheManager>().ImplementedBy<RedisCacheManager>().DependsOn(Dependency.OnValue<RedisItemConfig>(redisItemConfig), Dependency.OnValue<IRedisConnectionWrapper>(_iocResolver.Resolve<IRedisConnectionWrapper>($"{redisItemConfig.Name}_connection"))).LifestyleSingleton());
                }
            }
        }
    }
}