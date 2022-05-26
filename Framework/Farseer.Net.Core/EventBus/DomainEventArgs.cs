using System;
using FS.Extends;

namespace FS.Core.EventBus
{
    public class DomainEventArgs : EventArgs
    {
        public DomainEventArgs(object sender, object message)
        {
            Sender   = sender;
            Message  = message;
            Id       = Guid.NewGuid().ToString("N");
            CreateAt = DateTime.Now.ToTimestamps();
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 事件的发布时间
        /// </summary>
        public long CreateAt { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public object Message { get; set; }
        /// <summary>
        /// 发送者
        /// </summary>
        public object Sender { get; set; }
        
        /// <summary>
        /// 执行失败次数
        /// </summary>
        public int ErrorCount { get; set; }
    }
}