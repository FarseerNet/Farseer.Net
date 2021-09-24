using System.Collections.Generic;

namespace FS.Job.Configuration
{
    public class JobItemConfig
    {
        /// <summary>
        ///     调度平台的服务地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        ///     是否开启调试状态
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        ///     开启调试状态后要启动的job
        /// </summary>
        public string DebugJobs { get; set; }

        /// <summary>
        ///     开启调试状态后要启动的job
        /// </summary>
        public Dictionary<string, string> DebugMetaData { get; set; }
    }
}