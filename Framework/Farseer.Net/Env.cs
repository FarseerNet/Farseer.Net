using System;

namespace FS
{
    public class Env
    {
        /// <summary>
        ///     生产环境
        /// </summary>
        public static bool IsPro => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FS_ENV"));

        /// <summary>
        ///     测试环境
        /// </summary>
        public static bool IsTest => Environment.GetEnvironmentVariable("FS_ENV") == "test";

        /// <summary>
        ///     本地环境
        /// </summary>
        public static bool IsDev => Environment.GetEnvironmentVariable("FS_ENV") == "dev";

        /// <summary>
        ///     预发布环境
        /// </summary>
        public static bool IsPre => Environment.GetEnvironmentVariable("FS_ENV") == "pre";
    }
}