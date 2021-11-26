namespace FS.Job.Entity
{
    /// <summary>
    ///     任务状态
    /// </summary>
    public enum EumTaskType
    {
        /// <summary>
        ///     未开始
        /// </summary>
        None = 0,

        /// <summary>
        ///     已调度
        /// </summary>
        Scheduler = 1,

        /// <summary>
        ///     执行中
        /// </summary>
        Working = 2,

        /// <summary>
        ///     失败
        /// </summary>
        Fail = 3,

        /// <summary>
        ///     完成
        /// </summary>
        Success = 4
    }
}