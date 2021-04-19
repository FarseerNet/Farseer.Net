namespace FS.MQ.Rocket
{
    /// <summary>
    ///     生产消息
    /// </summary>
    public interface IHttpRocketConsumer
    {
        /// <summary>
        ///     消费订阅
        /// </summary>
        /// <param name="listen">消息监听处理</param>
        /// <param name="tag">标签</param>
        void Start(HttpMessageListener listen, string tag = "*");

        /// <summary>
        ///     关闭订阅消费
        /// </summary>
        void Close();
    }
}