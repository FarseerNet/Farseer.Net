using FS.Data.Log.Default;

namespace FS.Data.Log
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public static class LogManger
    {
        /// <summary>
        /// 日志管理
        /// </summary>
        public static ILog Log { get; private set; } = new DefaultLog();

        /// <summary>
        /// 使用外部实现方式
        /// </summary>
        /// <param name="log"></param>
        public static void Set(ILog log)
        {
            Log = log;
        }
    }
}
