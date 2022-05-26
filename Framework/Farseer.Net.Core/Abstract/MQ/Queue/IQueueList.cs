using System.Collections.Generic;

namespace FS.Core.Abstract.MQ.Queue;

/// <summary>
 /// 队列数据
 /// </summary>
public interface IQueueList 
{
    /// <summary>
    /// 根据配置中的拉取数量来分割每个小集合
    /// </summary>
    void CheckAndMoveQueue();
    /// <summary>
    /// 添加数据
    /// </summary>
    void Add(object data);
    /// <summary>
    /// 添加数据
    /// </summary>
    void Add(List<object> datalist);
    /// <summary>
    /// 获取当前队列大小
    /// </summary>
    int GetCurCount { get; }
    /// <summary>
    /// 拉取数据
    /// </summary>
    List<object> Pull();
}