using System;
using FS.Extends;

namespace FS.Core.LinkTrack
{
    /// <summary>
    /// 本次操作完成
    /// </summary>
    public class TrackEnd : IDisposable
    {
        private readonly LinkTrackDetail  _linkTrackDetail;
        private readonly LinkTrackContext _linkTrackContext;

        public TrackEnd(LinkTrackContext linkTrackContext)
        {
            _linkTrackContext = linkTrackContext;
        }

        public TrackEnd(LinkTrackDetail linkTrackDetail)
        {
            _linkTrackDetail = linkTrackDetail;
        }

        /// <summary>
        /// 设置API响应内容
        /// </summary>
        public void SetResponseBody(string responseBody)
        {
            _linkTrackContext.ResponseBody = responseBody;
        }

        /// <summary>
        /// 完成本次追踪
        /// </summary>
        public void Dispose()
        {
            if (_linkTrackDetail != null) _linkTrackDetail.EndTs   = DateTime.Now.ToTimestamps();
            if (_linkTrackContext != null) _linkTrackContext.EndTs = DateTime.Now.ToTimestamps();
        }
    }
}