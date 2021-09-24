using System;
using System.Reflection;
using FS.DI;
using FS.Modules;
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
            token.RegisterChangeCallback(callback: _ =>
            {
                changeTokenConsumer();
                OnChange(changeTokenProducer: changeTokenProducer, changeTokenConsumer: changeTokenConsumer);
            }, state: new object());
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new DataInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}