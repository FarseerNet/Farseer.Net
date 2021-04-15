//------------------------------------------------------------------------------
// <copyright file="FarseerKernelModule.cs" company="Farseer">
//     Copyright (c) Shanghai Zhongtongji Network Technology Co., Ltd.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * FarseerKernelModule.cs
 * 
 * Copyright (c) 2016-2030, Farseer Corporation
 * 
 */

using System.Reflection;
using FS.Configuration;
using FS.Configuration.Startup;
using FS.Core.Configuration;
using FS.DI;
using FS.Modules;

namespace FS.Core
{
    /// <summary>
    ///     系统核心模块
    /// </summary>
    public sealed class FarseerCoreModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            // 如果Redis配置没有创建，则创建它
            InitConfig(IocManager.Resolve<IConfigResolver>());
        }

        /// <inheritdoc />
        private void InitConfig(IConfigResolver configResolver)
        {
            // 全局配置
            var globalConfig = configResolver.Get<GlobalConfig>();
            if (globalConfig == null)
            {
                configResolver.Set(new GlobalConfig { AppName = "", ApiGatewayType = EumApiGatewayType.External });
                configResolver.Save();
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            foreach (var replaceAction in ((FarseerStartupConfiguration)Configuration).ServiceReplaceActions.Values) { replaceAction(); }
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}