using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FS.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     注册所有接口到.net core IOC中
        /// </summary>
        /// <param name="services"> </param>
        public static IServiceCollection AddFarseerIoc(this IServiceCollection services)
        {
            var ioc = IocManager.Instance;
            // 注册IServiceCollection到Farseer Ioc
            ioc.Container.Register(Component.For<IServiceCollection>().Instance(services).LifestyleSingleton());

            // 注册到IServiceCollection
            RegisterToServiceCollection(services, ioc);

            // 注册到Farseer Ioc
            RegisterToFarseer(services, ioc);
            return services;
        }

        /// <summary>
        /// 注册到IServiceCollection
        /// </summary>
        private static void RegisterToServiceCollection(IServiceCollection services, IocManager ioc)
        {
            // 注册Farseer Ioc到IServiceCollection
            services.AddSingleton<IIocManager>(implementationInstance: ioc);
            services.AddSingleton<IConfigurationRoot>(ioc.Resolve<IConfigurationRoot>());

            // 获取业务实现类
            foreach (var model in ioc.GetCustomComponent())
            {
                var serviceType = model.Services.FirstOrDefault(predicate: o => o.IsInterface) ?? model.Services.FirstOrDefault();
                // 没有注册到接口
                if (model.Implementation == serviceType)
                {
                    switch (model.LifestyleType)
                    {
                        case LifestyleType.Singleton:
                            services.AddSingleton(serviceType: model.Implementation);
                            break;
                        case LifestyleType.Transient:
                            services.AddTransient(serviceType: model.Implementation);
                            break;
                        case LifestyleType.Scoped:
                            services.AddScoped(serviceType: model.Implementation);
                            break;
                    }
                }
                else
                {
                    switch (model.LifestyleType)
                    {
                        case LifestyleType.Singleton:
                            services.AddSingleton(serviceType: serviceType, implementationType: model.Implementation);
                            break;
                        case LifestyleType.Transient:
                            services.AddTransient(serviceType: serviceType, implementationType: model.Implementation);
                            break;
                        case LifestyleType.Scoped:
                            services.AddScoped(serviceType: serviceType, implementationType: model.Implementation);
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// 注册到Farseer Ioc
        /// </summary>
        private static void RegisterToFarseer(IServiceCollection services, IocManager ioc)
        {
            var provider = services.BuildServiceProvider();
            ioc.Container.Register(Component.For<IServiceProvider>().Instance(provider).LifestyleSingleton());
            /*
            foreach (var service in services)
            {
                try
                {
                    switch (service.ServiceType.Assembly.ManifestModule.Name)
                    {
                        case "Microsoft.Extensions.Options.dll":
                        case "Microsoft.Extensions.Logging.dll":
                        case "Microsoft.AspNetCore.SignalR.dll":
                            continue;
                    }
                    if (ioc.IsRegistered(service.ServiceType)) continue;
                    switch (service.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            ioc.Container.Register(Component.For(service.ServiceType).Named(service.ServiceType.ToString()).Instance(provider.GetService(service.ServiceType)).LifestyleSingleton());
                            break;
                        case ServiceLifetime.Scoped:
                            ioc.Container.Register(Component.For(service.ServiceType).ImplementedBy(service.ImplementationType).LifestyleScoped());
                            break;
                        case ServiceLifetime.Transient:
                            ioc.Container.Register(Component.For(service.ServiceType).ImplementedBy(service.ImplementationType).LifestyleTransient());
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($".NET CORE的组件注册到Farseer.Net失败：{e.Message}");
                }
            }
            */
        }
    }
}