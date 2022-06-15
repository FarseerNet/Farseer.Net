using System;
using FS.DI;
using FS.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Farseer.Net.AspNetCore;

public static class AddFarseerControllersExtensions
{
    /// <summary>
    ///     实现asp.net core控制器IOC使用Castle代替
    /// </summary>
    public static void AddFarseerControllers(this IServiceCollection services, Action<MvcOptions> configure = null)
    {
        configure ??= o =>
        {
            o.Filters.Add(item: new BadRequestException());
        };
        services.Configure<KestrelServerOptions>(config: IocManager.GetService<IConfigurationRoot>().GetSection(key: "Kestrel"));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddFarseerLogging();
        
        services.AddFarseerIoc();
        
        // 注册Controllers
        services.AddControllers(configure: configure)
                .AddControllersAsServices()
                .AddNewtonsoftJson();
        
        services.AddFarseerNetApi();
    }
}