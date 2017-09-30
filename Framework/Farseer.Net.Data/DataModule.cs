﻿using System.Collections.Generic;
using System.Reflection;
using Farseer.Net.Configuration;
using Farseer.Net.Data.Configuration;
using Farseer.Net.DI;
using Farseer.Net.Modules;

namespace Farseer.Net.Data
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
            // 如果Db配置没有创建，则创建它
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        private void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.DbConfig();
            if (config == null)
            {
                configResolver.Set(new DbConfig { Items = new List<DbItemConfig> { new DbItemConfig { Name = "testDb", Server = "127.0.0.1" } } });
                configResolver.Save();
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new DataInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}