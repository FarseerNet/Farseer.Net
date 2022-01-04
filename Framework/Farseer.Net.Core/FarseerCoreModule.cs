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
using FS.Configuration.Startup;
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
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            foreach (var replaceAction in ((FarseerStartupConfiguration)StartupConfiguration).ServiceReplaceActions.Values) replaceAction();
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}