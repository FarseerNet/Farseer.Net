﻿namespace FS.DI
{
    /// <summary>
    ///     约定注册配置
    /// </summary>
    public class ConventionalRegistrationConfig
    {
        /// <summary>
        ///     构造函数（默认自动注册）
        /// </summary>
        public ConventionalRegistrationConfig()
        {
            InstallInstallers = true;
        }

        /// <summary>
        ///     自动注册实现类
        /// </summary>
        public bool InstallInstallers { get; set; }
    }
}