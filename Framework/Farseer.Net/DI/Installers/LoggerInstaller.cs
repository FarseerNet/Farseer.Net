using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.DI.Installers
{
    /// <summary>
    ///     日志组件注册
    /// </summary>
    public class LoggerInstaller : IWindsorInstaller
    {
        private readonly Action<ILoggingBuilder> _configure;

        public LoggerInstaller(Action<ILoggingBuilder> configure)
        {
            _configure = configure ?? (builder => { builder.AddFarseerJsonConsole(); });
        }

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 注册配置文件
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",                                                                 optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true) //增加环境配置文件
                .AddEnvironmentVariables()
                .Build();
            container.Register(Component.For<IConfigurationRoot>().Instance(configuration).LifestyleSingleton());

            // 注册默认日志组件
            var loggerFactory = LoggerFactory.Create(o =>
            {
                _configure(o);
                o.AddConfiguration(configuration.GetSection("Logging"));
            });
            container.Register(Component.For<ILoggerFactory>().Instance(loggerFactory).LifestyleSingleton());
        }
    }
}