﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Grpc.Configuration;

namespace FS.Grpc
{
    /// <summary>
    ///     IOC注册
    /// </summary>
    public class GrpcInstaller : IWindsorInstaller
    {
        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            using var grpcItemConfigs = GrpcConfigRoot.Get();

            foreach (var grpcItemConfig in grpcItemConfigs) container.Register(Component.For<IGrpcClient>().Named(name: grpcItemConfig.Name).ImplementedBy<GrpcClient>().DependsOn(dependency: Dependency.OnValue<GrpcItemConfig>(value: grpcItemConfig)).LifestyleSingleton());
        }
    }
}