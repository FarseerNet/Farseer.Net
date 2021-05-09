using System.Threading.Tasks;

namespace FS.MQ.RedisStream
{
    public interface IRedisStreamProduct
    {
        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message">消息主体</param>
        bool Send(string message, string field = "data");

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="message">消息主体</param>
        Task<bool> SendAsync(string message, string field = "data");

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity">消息主体</param>
        bool Send(object entity, string field = "data");

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="field"> </param>
        /// <param name="entity">消息主体</param>
        Task<bool> SendAsync(object entity, string field = "data");
    }
}