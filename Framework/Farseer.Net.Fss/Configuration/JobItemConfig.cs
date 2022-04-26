
namespace FS.Fss.Configuration
{
    public class JobItemConfig
    {
        /// <summary>
        ///     调度平台的服务地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        ///     一次拉取的任务数量
        /// </summary>
        public int PullCount { get; set; }

        /// <summary>
        ///     允许当前正在执行的任务数量
        /// </summary>
        public int WorkCount { get; set; }
    }
}