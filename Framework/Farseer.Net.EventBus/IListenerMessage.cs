using System.Threading.Tasks;

namespace FS.EventBus
{
    /// <summary>
    ///     事件总线监听消费
    /// </summary>
    public interface IListenerMessage
    {
        /// <summary>
        ///     消费
        /// </summary>
        /// <returns> 当开启手动确认时，返回true时，才会进行ACK确认 </returns>
        Task<bool> Consumer(string message, object sender, DomainEventArgs ea);
    }
}