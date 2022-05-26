using System.Collections.Generic;
using FS.Core.Abstract.MQ.Queue;
using FS.DI;
using FS.MQ.Queue.Configuration;

namespace FS.MQ.Queue
{
    public class QueueProduct : IQueueProduct
    {
        /// <summary>
        ///     配置信息
        /// </summary>
        private readonly QueueConfig _queueConfig;
        /// <summary>
        /// 队列数据
        /// </summary>
        private readonly IQueueList _queueList;
        public QueueProduct(QueueConfig queueConfig)
        {
            if (queueConfig.MaxCount  == 0) queueConfig.MaxCount  = 1000000;
            if (queueConfig.PullCount == 0) queueConfig.PullCount = 1000;
            _queueConfig = queueConfig;

            _queueList = IocManager.GetService<IQueueList>($"queue_list_{queueConfig.Name}");
        }

        /// <summary>
        ///     发送数据
        /// </summary>
        /// <param name="data"> 数据 </param>
        public void Send(object data)
        {
            _queueList.CheckAndMoveQueue();
            _queueList.Add(data);
        }

        /// <summary>
        ///     发送数据
        /// </summary>
        /// <param name="datalist"> 数据 </param>
        public void Send(List<object> datalist)
        {
            // 如果当前集合 + 要添加的数据的大小，超出设置的拉取数量，则需要做集合切割
            if (_queueList.GetCurCount + datalist.Count > _queueConfig.PullCount)
            {
                _queueList.Add(datalist);
                _queueList.CheckAndMoveQueue();
            }
            else
            {
                _queueList.Add(datalist);
            }
        }
    }
}