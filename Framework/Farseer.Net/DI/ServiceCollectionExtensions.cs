using System;
using System.Linq;
using Castle.Core;
using Microsoft.Extensions.DependencyInjection;
namespace FS.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册所有接口到.net core IOC中
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddFarseerIoc(this IServiceCollection services)
        {
            var ioc = FS.DI.IocManager.Instance;
            services.AddSingleton<IIocManager>(ioc);
            
            // 获取业务实现类
            var lstModel = ioc.GetCustomComponent();
            for (int index = 0; index < lstModel.Count; index++)
            {
                var model        = lstModel[index];
                var serviceType = model.Services.FirstOrDefault(o => o.IsInterface) ?? model.Services.FirstOrDefault();
                // 没有注册到接口
                if (model.Implementation == serviceType)
                {
                    switch (model.LifestyleType)
                    {
                        case LifestyleType.Singleton:
                            services.AddSingleton(model.Implementation);
                            break;
                        case LifestyleType.Transient:
                            services.AddTransient(model.Implementation);
                            break;
                        case LifestyleType.Scoped:
                            services.AddScoped(model.Implementation);
                            break;
                    }
                }
                else
                {
                    switch (model.LifestyleType)
                    {
                        case LifestyleType.Singleton:
                            services.AddSingleton(serviceType,model.Implementation);
                            break;
                        case LifestyleType.Transient:
                            services.AddTransient(serviceType, model.Implementation);
                            break;
                        case LifestyleType.Scoped:
                            services.AddScoped(serviceType, model.Implementation);
                            break;
                    }
                }
            }

            return services;
        }
    }
}