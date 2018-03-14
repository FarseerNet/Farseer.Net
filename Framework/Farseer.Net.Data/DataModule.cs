//------------------------------------------------------------------------------
// <copyright file="DbModule.cs" company="ZTO">
//     Copyright (c) Shanghai Zhongtongji Network Technology Co., Ltd.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * DbModule.cs
 * 
 * Copyright (c) 2016-2030, ZTO Corporation
 * 
 */

using System.Collections.Generic;
using System.Reflection;
using FS.Configuration;
using FS.Data.Configuration;
using FS.DI;
using FS.Modules;
using FS.Configuration;

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
            // 如果Db配置没有创建，则创建它
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        private void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.DbConfig();
            if (config == null)
            {
                configResolver.Set(config = new DbConfig { Items = new List<DbItemConfig> { new DbItemConfig { Name = "testDb", Server = "127.0.0.1" } } });
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