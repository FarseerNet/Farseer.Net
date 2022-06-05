namespace FS.Core.Abstract.MQ;

/// <summary>
/// 所有MQ生产者的基类
/// </summary>
public interface IMqProduct
{
    /// <summary>
    /// 当前队列名称
    /// </summary>
    string QueueName { get; }
}