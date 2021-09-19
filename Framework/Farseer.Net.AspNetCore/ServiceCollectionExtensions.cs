using System;
using FS.DI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FS
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///  实现asp.net core控制器IOC使用Castle代替
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IMvcBuilder AddFarseerControllers(this IServiceCollection services, Action<MvcOptions> configure = null)
        {
            configure ??= o => { o.Filters.Add(new BadRequestException()); };
            services.Configure<KestrelServerOptions>(FS.DI.IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("Kestrel"));
            services.AddFarseerIoc();
            return services.AddControllers(configure)
                .AddControllersAsServices()
                .AddNewtonsoftJson();
        }
    }
}