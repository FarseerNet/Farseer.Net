// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-23 22:50
// ********************************************

using System.Collections.Generic;
using FS.Core.Abstract.MQ.Queue;
using FS.MQ.Queue.Configuration;

namespace FS.MQ.Queue
{
    /// <summary>
    ///     Rabbit管理器
    /// </summary>
    public class QueueManager : IQueueManager
    {
        private static readonly object ObjLock = new();

        /// <summary>
        ///     配置信息
        /// </summary>
        private readonly QueueConfig _productConfig;

        /// <summary>
        ///     生产消息
        /// </summary>
        private IQueueProduct _product;

        /// <summary> Rabbit管理器 </summary>
        public QueueManager(QueueConfig productConfig)
        {
            _productConfig = productConfig;
        }

        /// <summary>
        ///     生产普通消息
        /// </summary>
        public IQueueProduct Product
        {
            get
            {
                if (_product != null) return _product;
                lock (ObjLock)
                {
                    return _product ??= new QueueProduct(queueConfig: _productConfig);
                }
            }
        }

    }
}