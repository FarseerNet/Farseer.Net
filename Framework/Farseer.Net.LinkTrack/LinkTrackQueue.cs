using System.Collections.Generic;
using System.Linq;
using FS.Core.Async;
using FS.Core.LinkTrack;

namespace FS.LinkTrack
{
    public class LinkTrackQueue : BaseAsyncQueue<LinkTrackContext>
    {
        private static LinkTrackQueue _instance;
        public static LinkTrackQueue Instance => _instance ??= new LinkTrackQueue();

        private LinkTrackQueue() : base(500000, 100, 500)
        {
        }

        /// <summary>
        ///     回调 数据给用户处理
        /// </summary>
        /// <param name="lst">回调的 数据列表</param>
        /// <param name="remainCount">队列中当前剩余多少要处理</param>
        protected override void OnDequeue(List<LinkTrackContext> lst, int remainCount)
        {
            LinkTrackEsContext.Data.LinkTrackContext.Insert(lst.Select(o => new LinkTrackContextPO
            {
                AppId     = o.AppId,
                ContextId = o.ContextId,
                List      = o.List,
                Id        = $"{o.AppId}_{o.ContextId}"
            }).ToList());
        }

        /// <summary>
        /// 将链路追踪写入队列
        /// </summary>
        public static void Enqueue() => Instance.Enqueue(FsLinkTrack.Current.Get());
    }
}