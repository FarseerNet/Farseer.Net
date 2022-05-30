using FS.Core.Abstract.EventBus;
using FS.DI;

namespace FS.Core.DomainDriven.DomainEvent;

/// <summary>
/// 领域事件的基类
/// </summary>
public abstract class BaseDomainEvent
{
    /// <summary>
    /// 事件名称
    /// </summary>
    protected abstract string EventName { get; }

    /// <summary>
    /// 发布事件
    /// </summary>
    public virtual void PublishEvent()
    {
        IocManager.GetService<IEventProduct>(EventName).SendSync(this, this);
    }
}