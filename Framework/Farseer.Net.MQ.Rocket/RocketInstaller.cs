﻿using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.MQ.Rocket.Configuration;
using Microsoft.Extensions.Configuration;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     RocketMQ的IOC注册
    /// </summary>
    public class RocketInstaller : IWindsorInstaller
    {
        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("Rocket");
            var rocketItemConfigs    = configurationSection.GetChildren().Select(o => o.Get<RocketItemConfig>()).ToList();

            //注册所有的消息队列的Topic消费者
            rocketItemConfigs.ForEach(c =>
                container.Register(Component.For<IRocketManager>().Named(c.Name).ImplementedBy<RocketManager>()
                    .DependsOn(Dependency.OnValue<RocketItemConfig>(c)).LifestyleSingleton()));
        }
    }
}