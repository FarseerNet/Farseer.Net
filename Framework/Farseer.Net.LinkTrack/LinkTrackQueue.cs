using FS.Core.Abstract.MQ.Queue;
using FS.Core.LinkTrack;
using FS.DI;

namespace FS.LinkTrack
{
    public class LinkTrackQueue : ILinkTrackQueue
    {
        readonly IQueueProduct _queueProduct;
        public LinkTrackQueue()
        {
            _queueProduct = IocManager.GetService<IQueueProduct>(name: "LinkTrackQueue");
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