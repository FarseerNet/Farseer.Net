using Farseer.Net.Configuration;

namespace Farseer.Net.Log.Configuration
{
    public static class NLogConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static NLogConfig NLogConfig(this IConfigResolver resolver) => resolver.Get<NLogConfig>();
    }
}
