using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Farseer.Net.AspNetCore.DynamicApi;
using Farseer.Net.AspNetCore.Filters;
using FS;
using FS.Core.Abstract.AspNetCore;
using FS.DI;
using FS.Log;
using FS.Modules;
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
    public static void AddFarseerControllers(this IServiceCollection services, string appName, Action<MvcOptions> configure = null)
    {
        // 查找启动模块，并初始化Farseer.Net
        var farseerModuleType = typeof(FarseerModule);
        var startupModuleType = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(t => t.BaseType == farseerModuleType || t.GetInterfaces().Contains(farseerModuleType));
        if (startupModuleType == null) throw new FarseerException($"未在启动的程序集，找到启动模块，该模块应该是继承：FarseerModule的类");
        FarseerApplication.Run(startupModuleType, appName).Initialize();

        configure ??= o =>
        {
            o.Filters.Add(item: new BadRequestException());
        };
        services.Configure<KestrelServerOptions>(config: IocManager.GetService<IConfigurationRoot>().GetSection(key: "Kestrel"));

        IocManager.Instance.Register<IHttpContextAccessor, HttpContextAccessor>();
        IocManager.Instance.Register<IDynamicWebApiOptions, DynamicWebApiOptions>();
        services.AddFarseerLogging();

        services.AddFarseerIoc();

        // 注册Controllers
        services.AddControllers(configure: configure)
                .AddControllersAsServices()
                .AddNewtonsoftJson();

        services.AddFarseerNetApi();
    }
}