using System.Collections.Generic;

namespace FS.EventBus
{
    public interface IEventProduct
    {
        /// <summary>
        ///     发送事件消息
        /// </summary>
        /// <param name="sender">事件发布者 </param>
        /// <param name="message"> 消息主体 </param>
        bool Send(object sender, string message);
        /// <summary>
        /// 发送事件消息（订阅方以异步的方式执行，当前方法发送完后立即返回结果）
        /// </summary>
        /// <param name="sender">事件发布者 </param>
        /// <param name="message"> 消息主体 </param>
        /// <returns></returns>
        bool SendAsync(object sender, string message);
    }
}