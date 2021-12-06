using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.Async;
using FS.Core.LinkTrack;
using FS.EC.Dal;
using FS.Extends;

namespace FS.EC
{
    public class EnvCollectQueue : BaseAsyncQueue<LinkTrackContext>, ILinkTrackQueue
    {
        internal EnvCollectQueue() : base(maxQueueSize: 500000, callBackListCapacity: 1000, sleepMs: 500)
        {
        }

        /// <summary>
        ///     将链路追踪写入队列
        /// </summary>
        public void Enqueue() => base.Enqueue(obj: FsLinkTrack.Current.Get());

        /// <summary>
        ///     回调 数据给用户处理
        /// </summary>
        /// <param name="lst"> 回调的 数据列表 </param>
        /// <param name="remainCount"> 队列中当前剩余多少要处理 </param>
        protected override Task OnDequeue(List<LinkTrackContext> lst, int remainCount)
        {
            // 设置C#的调用链
            foreach (var linkTrackContext in lst) linkTrackContext.List.ForEach(action: o => o.SetCallStackTrace());

            return Task.WhenAll(
                                EnvCollectEsContext.Data.Host.InsertAsync(lst.Select(o => o.Map<HostPO>()).ToList())
                                );
        }
    }
}