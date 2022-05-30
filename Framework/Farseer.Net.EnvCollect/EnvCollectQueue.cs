using FS.Core.Abstract.MQ.Queue;
using FS.Core.LinkTrack;
using FS.DI;

namespace FS.EC
{
    public class EnvCollectQueue : IEnvCollectQueue
    {
        readonly IQueueProduct _queueProduct;
        internal EnvCollectQueue()
        {
            _queueProduct = IocManager.GetService<IQueueProduct>(name: "EnvCollect");
        }

        /// <summary>
        ///     将链路追踪写入队列
        /// </summary>
        public void Enqueue()
        {
            _queueProduct.Send(FsLinkTrack.Current.Get());
        }
    }
}