using System;

namespace FS.Utils.Common
{
    /// <summary>
    ///     获取系统路径
    /// </summary>
    public static class SysMapPath
    {
        /// <summary>
        ///     获取项目的App_Data的路径
        /// </summary>
        public static string AppData { get; set; } = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\";

        /// <summary>
        /// 日志的路径保存位置
        /// </summary>
        public static string LogPath => SysMapPath.AppData + "log/";

        /// <summary>
        /// Debug的路径保存位置
        /// </summary>
        public static string DebugPath => SysMapPath.AppData + "log/Debug/";

        /// <summary>
        /// Error的路径保存位置
        /// </summary>
        public static string ErrorPath => SysMapPath.AppData + "log/Error/";

        /// <summary>
        /// Fatal的路径保存位置
        /// </summary>
        public static string FatalPath => SysMapPath.AppData + "log/Fatal/";

        /// <summary>
        /// Info的路径保存位置
        /// </summary>
        public static string InfoPath => SysMapPath.AppData + "log/Info/";

        /// <summary>
        /// Warn的路径保存位置
        /// </summary>
        public static string WarnPath => SysMapPath.AppData + "log/Warn/";

        /// <summary>
        /// Sql错误的路径保存位置
        /// </summary>
        public static string SqlErrorPath => SysMapPath.AppData + "log/SqlError/";

        /// <summary>
        /// Sql运行的路径保存位置
        /// </summary>
        public static string SqlRunPath => SysMapPath.AppData + "log/SqlRun/";
    }
}