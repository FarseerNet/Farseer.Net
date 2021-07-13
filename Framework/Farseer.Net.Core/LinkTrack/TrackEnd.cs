using System;
using FS.Extends;

namespace FS.Core.LinkTrack
{
    /// <summary>
    /// 本次操作完成
    /// </summary>
    public class TrackEnd : IDisposable
    {
        private readonly LinkTrackDetail _linkTrackDetail;

        public TrackEnd(LinkTrackDetail linkTrackDetail)
        {
            _linkTrackDetail = linkTrackDetail;
        }

        /// <summary>
        /// 完成本次追踪
        /// </summary>
        public void End()
        {
            _linkTrackDetail.EndTs = DateTime.Now.ToTimestamps();
        }

        public void Dispose()
        {
            End();
        }
    }
}