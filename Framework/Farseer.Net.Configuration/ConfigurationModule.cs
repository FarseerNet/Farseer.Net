// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 11:01
// ********************************************

using Farseer.Net.Modules;
using Farseer.Net.Reflection;

namespace Farseer.Net.Configuration
{
    /// <summary>
    /// 配置文件模块管理
    /// </summary>
    public class ConfigurationModule : FarseerModule
    {
        private IConfigResolver _configResolver;

        /// <summary>
        /// 扫描所有继承FarseerConfig类的配置文件
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.Container.Install(new ConfigurationInstaller());

            // 取出本地配置解析器
            _configResolver = IocManager.Resolve<IConfigResolver>("LocalConfigResolver");

            // 扫描所有继承FarseerConfig类的配置文件
            ScanAppendFarseerConfig();

            // 加载配置文件
            _configResolver.Load();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public override void Initialize() { }

        /// <summary>
        /// 扫描所有继承FarseerConfig类的配置文件
        /// </summary>
        private void ScanAppendFarseerConfig()
        {
            var configTypes = IocManager.Resolve<ITypeFinder>().Find(o => !o.IsAbstract && typeof(IFarseerConfig).IsAssignableFrom(o));
            if (configTypes == null) { return; }

            // 追加到解析器
            foreach (var type in configTypes) { _configResolver.Append(type); }
        }
    }
}