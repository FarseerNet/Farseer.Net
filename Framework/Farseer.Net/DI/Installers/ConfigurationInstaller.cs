using System;
using System.Diagnostics;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Extensions.Configuration;

namespace FS.DI.Installers;

/// <summary>
///     配置组件注册
/// </summary>
public class ConfigurationInstaller : IWindsorInstaller
{
    /// <summary>
    ///     注册
    /// </summary>
    /// <param name="container"> 容器 </param>
    /// <param name="store"> </param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        // 注册配置文件 耗时：250 ms（优化后：150ms）
        var configuration = new ConfigurationBuilder()
                            .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true)
                            // 通过环境加载，会额外多出约100ms的损耗，这里取消环境源，推荐使用环境变量替换
                            //.AddJsonFile(path: $"appsettings.{Environment.GetEnvironmentVariable(variable: "ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true) //增加环境配置文件
                            .AddEnvironmentVariables()
                            .Build();
            
        container.Register(Component.For<IConfigurationRoot>().Instance(instance: configuration).LifestyleSingleton());
    }
}