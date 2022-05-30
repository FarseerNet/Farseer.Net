using System;
using System.Diagnostics;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.DI.Installers;

/// <summary>
///     日志组件注册
/// </summary>
public class LoggerInstaller : IWindsorInstaller
{
    /// <summary>
    ///     注册
    /// </summary>
    /// <param name="container"> 容器 </param>
    /// <param name="store"> </param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        // 注册默认日志组件
        var loggerFactory = LoggerFactory.Create(configure: o =>
        {
            o.AddConsole();
            o.AddDebug();
        });
        container.Register(Component.For<ILoggerFactory>().Instance(instance: loggerFactory).LifestyleSingleton());
    }
}