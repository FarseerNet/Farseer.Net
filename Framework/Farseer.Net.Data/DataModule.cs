using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.MicroKernel.Registration;
using FS.Configuration;
using FS.Data.Client;
using FS.Data.Configuration;
using FS.Data.Internal;
using FS.DI;
using FS.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace FS.Data
{
    /// <summary>
    ///     Db模块
    /// </summary>
    public class DataModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }
        
        public static void OnChange(Func<IChangeToken> changeTokenProducer, Action changeTokenConsumer)
        {
            var token = changeTokenProducer();
            token.RegisterChangeCallback(_ =>
            {
                changeTokenConsumer();
                OnChange(changeTokenProducer, changeTokenConsumer);
            }, new object());
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new DataInstaller());
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}