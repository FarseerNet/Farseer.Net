using System;
using System.Linq;
using Farseer.Net.AspNetCore.DynamicApi;
using FS.Core.Abstract.AspNetCore;
using FS.DI;
using FS.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Farseer.Net.AspNetCore;

/// <summary>
/// 使用Farseer.Net动态API
/// </summary>
public static class AddFarseerNetApiExtensions
{
    /// <summary>
    ///     实现动态API
    /// </summary>
    public static IServiceCollection AddFarseerNetApi(this IServiceCollection services)
    {
        // 找到应用管理
        var applicationPartManager = services.GetService<ApplicationPartManager>();

        // 添加判断是否控制器的提供
        applicationPartManager.FeatureProviders.Add(new DefaultControllerFeatureProvider());

        // 找到使用了UseApi特性的类
        var typeFinder    = IocManager.GetService<ITypeFinder>();
        var findAttribute = typeFinder.FindAttribute<UseApiAttribute>();

        // 将使用了UseApi的程序集，加入到ApplicationParts中
        foreach (var assembly in findAttribute.Select(o => o.Assembly).Distinct())
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (var part in partFactory.GetApplicationParts(assembly))
            {
                applicationPartManager.ApplicationParts.Add(part);
            }
        }

        services.Configure<MvcOptions>(o =>
        {
            // 添加控制器路由的约定
            o.Conventions.Add(new DynamicWebApiConvention());
        });

        return services;
    }

    /// <summary>
    ///     实现动态API
    /// </summary>
    public static IApplicationBuilder AddFarseerNetApi(this IApplicationBuilder application)
    {
        // 找到应用管理
        var applicationPartManager = IocManager.GetService<ApplicationPartManager>();
        // 添加判断是否控制器的提供
        applicationPartManager.FeatureProviders.Add(new DefaultControllerFeatureProvider());

        // 找到使用了UseApi特性的类
        var typeFinder    = IocManager.GetService<ITypeFinder>();
        var findAttribute = typeFinder.FindAttribute<UseApiAttribute>();
        if (findAttribute == null) return application;

        // 将使用了UseApi的程序集，加入到ApplicationParts中
        foreach (var assembly in findAttribute.Select(o => o.Assembly).Distinct())
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (var part in partFactory.GetApplicationParts(assembly))
            {
                applicationPartManager.ApplicationParts.Add(part);
            }
        }

        var mvcOptions = application.ApplicationServices.GetRequiredService<IOptions<MvcOptions>>();

        // 添加控制器路由的约定
        mvcOptions.Value.Conventions.Add(new DynamicWebApiConvention());

        return application;
    }

    /// <summary>
    ///     配置动态API
    /// </summary>
    public static IApplicationBuilder ConfigureApi(this IApplicationBuilder application, Action<IDynamicWebApiOptions> options)
    {
        var dynamicWebApiOptions = IocManager.GetService<IDynamicWebApiOptions>();
        options(dynamicWebApiOptions);
        return application;
    }
}