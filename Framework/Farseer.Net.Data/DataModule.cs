﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.Mapping.Attribute;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.DI;
using FS.Modules;
using FS.Reflection;
using Microsoft.Extensions.Logging;

namespace FS.Data
{
    /// <summary>
    ///     Db模块
    /// </summary>
    public class DataModule : FarseerModule
    {
        private readonly ITypeFinder _typeFinder;

        public DataModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }
        
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
            IocManager.Container.Install(new DataInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });

            Task.Run(() =>
            {
                // 找到Context
                var lstContext = _typeFinder.Find(o => !o.IsGenericType && o.IsClass && o.BaseType != null && o.BaseType.BaseType != null && o.BaseType.BaseType == typeof(DbContext));

                foreach (var context in lstContext)
                {
                    // 需要做实例化，才能初始化上下文
                    Activator.CreateInstance(context);
                    // 找到Set属性
                    var set       = context.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    var lstPOType = set.Select(o => o.PropertyType).Where(o => o.GetInterfaces().Any(i => i == typeof(IDbSet)));
                    if (lstPOType != null)
                    {
                        var lstLog = new List<string>() { $"开启异步编译{context.Name}上下文中的{lstPOType.Count()}个实体类" };
                        var sw     = Stopwatch.StartNew();
                        foreach (var setType in lstPOType)
                        {
                            var beginIndex = sw.ElapsedMilliseconds;
                            new EntityDynamics().BuildType(setType.GenericTypeArguments[0]);
                            lstLog.Add($"编译：{setType.GenericTypeArguments[0].FullName} \t耗时：{(sw.ElapsedMilliseconds - beginIndex):n} ms");
                        }
                        lstLog.Add($"编译共耗时：{sw.ElapsedMilliseconds:n} ms");
                        IocManager.Logger<FarseerApplication>().LogDebug(string.Join("\r\n", lstLog));
                    }
                }
            });
        }
    }
}