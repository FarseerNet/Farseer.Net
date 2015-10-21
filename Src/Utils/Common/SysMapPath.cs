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
    }
}