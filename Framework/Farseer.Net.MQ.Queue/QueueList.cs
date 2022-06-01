using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS.Core.Abstract.MQ.Queue;
using FS.MQ.Queue.Configuration;

namespace FS.MQ.Queue;

public class QueueList : IQueueList
{
    private readonly object      _objLock = new();
    private readonly QueueConfig _queueConfig;
    /// <summary>
    /// _curQueue在_queues的逻辑位置
    /// </summary>
    private int _queueEmptyIndex;
    /// <summary>
    /// 如果_curQueue数量==设置的每次拉取数量，则会将_curQueue添加到_queues中。
    /// 在_queues数组中，每个数组的PooledList数量<= queueConfig.PullCount
    /// </summary>
    private readonly PooledList<object>[] _queues;
    /// <summary>
    /// 当前正在发送的数据，如果数量==queueConfig.PullCount，则会把当前的队列的数据，全部移到_queues
    /// </summary>
    private PooledList<object> _curQueue;

    public QueueList(QueueConfig queueConfig)
    {
        if (queueConfig.MaxCount == 0) queueConfig.MaxCount = 100000000;
        _queueConfig = queueConfig;
        _queues      = new PooledList<object>[(queueConfig.MaxCount / queueConfig.PullCount) + 1];
        _curQueue    = new(queueConfig.PullCount);
    }

    /// <summary>
    /// 根据配置中的拉取数量来分割每个小集合
    /// </summary>
    public void CheckAndMoveQueue()
    {
        if (_curQueue.Count >= _queueConfig.PullCount)
        {
            lock (_objLock)
            {
                // 正好相等，则直接将当前集合，添加到列表中
                if (_curQueue.Count == _queueConfig.PullCount)
                {
                    _queues[_queueEmptyIndex] = _curQueue;
                    _curQueue                 = new(_queueConfig.PullCount);
                    _queueEmptyIndex++;
                }
                else
                {
                    // 超出_productConfig.PullCount值，则通过截取的方式，保证每个集合不能超过_productConfig.PullCount
                    while (_curQueue.Count > _queueConfig.PullCount)
                    {
                        _queues[_queueEmptyIndex] = _curQueue.Take(_queueConfig.PullCount).ToPooledList();
                        // for (int i = 0; i < _queueConfig.PullCount; i++)
                        // {
                        //     CurQueue.RemoveAt(0);
                        // }
                        _curQueue.RemoveRange(0, _queueConfig.PullCount);
                        _queueEmptyIndex++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 添加数据
    /// </summary>
    public void Add(object data)
    {
        // 当前所有队列的总数 大于 允许的最大数量时，则丢弃最新的数据
        if (TotalCount >= _queueConfig.MaxCount) return;
        _curQueue.Add(data);
        TotalCount++;
    }

    /// <summary>
    /// 添加数据
    /// </summary>
    public void Add(IEnumerable<object> datalist)
    {
        // 当前所有队列的总数 大于 允许的最大数量时，则丢弃最新的数据
        if (_queueConfig.MaxCount > 0 && TotalCount >= _queueConfig.MaxCount) return;

        var totalCount = datalist.Count();
        // 当前所有队列的总数 + datalist数量 大于 允许的最大数量时，则丢弃最新的数据
        if (TotalCount + totalCount >= _queueConfig.MaxCount)
        {
            var surplusCount = _queueConfig.MaxCount - TotalCount;
            using var take         = datalist.Take(surplusCount).ToPooledQueue();
            _curQueue.AddRange(take);
            TotalCount += take.Count;
            return;
        }

        _curQueue.AddRange(datalist);
        TotalCount += totalCount;
    }

    /// <summary>
    /// 获取当前队列大小
    /// </summary>
    public int GetCurCount => _curQueue.Count;

    /// <summary>
    /// 获取总的队列大小
    /// </summary>
    public int TotalCount;

    /// <summary>
    /// 拉取数据
    /// </summary>
    public PooledList<object> Pull()
    {
        // 如果已经有满载的数据，则直接取出返回
        if (_queues[0] != null)
        {
            var queue = _queues[0];

            // 移动空元素
            for (int index = 0; index < _queueEmptyIndex; index++)
            {
                _queues[index] = _queues[index + 1];
            }

            _queueEmptyIndex--;
            return queue;
        }

        // 并没，则从当前队列中截取并返回
        var curCount = _curQueue.Count;
        if (curCount > 0)
        {
            var objects = _curQueue.Take(curCount).ToPooledList();
            // for (int i = 0; i < objects.Count; i++)
            // {
            //     CurQueue.RemoveAt(0);
            // }
            _curQueue.RemoveRange(0, objects.Count);
            TotalCount -= objects.Count;
            return objects;
        }

        return null;
    }
}