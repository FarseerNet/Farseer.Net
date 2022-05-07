namespace FS.MQ.Queue.Configuration
{
    public class QueueConfig
    {
        /// <summary> 队列名称 </summary>
        public string Name { get; set; }
        /// <summary>
        ///     每次拉取的队列数量。
        /// </summary>
        public int PullCount { get; set; }
        /// <summary>
        ///     队列允许写入的最大数量，超出后，不再写入队列中，0：不限制
        /// </summary>
        public int MaxCount { get; set; }
        /// <summary>
        ///     批量拉取消息时的间隔休眠时间 
        /// </summary>
        public int SleepTime { get; set; }
    }
}