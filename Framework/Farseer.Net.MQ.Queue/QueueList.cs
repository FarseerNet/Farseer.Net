using System.Linq;
using Collections.Pooled;
using FS.Core.Abstract.MQ.Queue;
using FS.MQ.Queue.Configuration;

namespace FS.MQ.Queue;

public class QueueList : IQueueList
{
    private readonly QueueConfig          _queueConfig;
    private readonly PooledList<object>[] Queues;
    private readonly object               objLock = new();
    private          PooledList<object>   CurQueue;
    private          int                  QueueEmptyIndex = 0;

    public QueueList(QueueConfig queueConfig)
    {
        if (queueConfig.MaxCount == 0) queueConfig.MaxCount = 100000000;
        _queueConfig = queueConfig;
        Queues       = new PooledList<object>[(queueConfig.MaxCount / queueConfig.PullCount) + 1];
        CurQueue     = new(queueConfig.PullCount);
    }

    /// <summary>
    /// 根据配置中的拉取数量来分割每个小集合
    /// </summary>
    public void CheckAndMoveQueue()
    {
        if (CurQueue.Count >= _queueConfig.PullCount)
        {
            lock (objLock)
            {
                // 正好相等，则直接将当前集合，添加到列表中
                if (CurQueue.Count == _queueConfig.PullCount)
                {
                    Queues[QueueEmptyIndex] = CurQueue;
                    CurQueue                = new(_queueConfig.PullCount);
                    QueueEmptyIndex++;
                }
                else
                {
                    // 超出_productConfig.PullCount值，则通过截取的方式，保证每个集合不能超过_productConfig.PullCount
                    while (CurQueue.Count > _queueConfig.PullCount)
                    {
                        Queues[QueueEmptyIndex] = CurQueue.Take(_queueConfig.PullCount).ToPooledList();
                        // for (int i = 0; i < _queueConfig.PullCount; i++)
                        // {
                        //     CurQueue.RemoveAt(0);
                        // }
                        CurQueue.RemoveRange(0, _queueConfig.PullCount);
                        QueueEmptyIndex++;
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
        CurQueue.Add(data);
        TotalCount++;
    }

    /// <summary>
    /// 添加数据
    /// </summary>
    public void Add(PooledList<object> datalist)
    {
        // 当前所有队列的总数 大于 允许的最大数量时，则丢弃最新的数据
        if (_queueConfig.MaxCount > 0 && TotalCount >= _queueConfig.MaxCount) return;

        // 当前所有队列的总数 + datalist数量 大于 允许的最大数量时，则丢弃最新的数据
        if (TotalCount + datalist.Count >= _queueConfig.MaxCount)
        {
            var surplusCount = _queueConfig.MaxCount - TotalCount;
            var take         = datalist.Take(surplusCount);
            CurQueue.AddRange(take);
            TotalCount += take.Count();
            return;
        }

        CurQueue.AddRange(datalist);
        TotalCount += datalist.Count;
    }

    /// <summary>
    /// 获取当前队列大小
    /// </summary>
    public int GetCurCount => CurQueue.Count;

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
        if (Queues[0] != null)
        {
            var queue = Queues[0];

            // 移动空元素
            for (int index = 0; index < QueueEmptyIndex; index++)
            {
                Queues[index] = Queues[index + 1];
            }

            QueueEmptyIndex--;
            return queue;
        }

        // 并没，则从当前队列中截取并返回
        var curCount = CurQueue.Count;
        if (curCount > 0)
        {
            var objects = CurQueue.Take(curCount).ToPooledList();
            // for (int i = 0; i < objects.Count; i++)
            // {
            //     CurQueue.RemoveAt(0);
            // }
            CurQueue.RemoveRange(0, objects.Count);
            TotalCount -= objects.Count;
            return objects;
        }

        return null;
    }
}