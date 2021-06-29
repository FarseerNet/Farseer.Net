using System;

namespace FS
{
    public class Env
    {
        /// <summary>
        ///     生产环境
        /// </summary>
        public static bool IsPro => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        /// <summary>
        ///     测试环境
        /// </summary>
        public static bool IsTest => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "test";

        /// <summary>
        ///     本地环境
        /// </summary>
        public static bool IsDev => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "dev";

        /// <summary>
        ///     预发布环境
        /// </summary>
        public static bool IsPre => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "pre";
    }
}