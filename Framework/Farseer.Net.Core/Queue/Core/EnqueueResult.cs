namespace FS.Core.Queue.Core
{
    /// <summary>
    /// 同步队列请求入队的结果
    /// </summary>
    public enum EnqueueResult
    {
        /// <summary>已处理完毕</summary>
        Sucess = 0,
        /// <summary>等待结果超时</summary>
        Timeout = 1,
        /// <summary>上一次的请求尚未完成</summary>
        Busy =  2,
        /// <summary>队列满了</summary>
        Overflow = 3
    }
}
