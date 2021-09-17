using System;
using System.Collections.Generic;
using FS.DI;
using FS.Extends;
using Microsoft.Extensions.Logging;

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
        /// 设置下游系统API响应内容
        /// </summary>
        public void SetDownstreamResponseBody(string responseBody)
        {
            _linkTrackContext.ResponseBody = responseBody;
        }

        /// <summary>
        /// 设置Http响应内容
        /// </summary>
        public void SetHttpResponseBody(string responseBody)
        {
            _linkTrackDetail.Data["ResponseBody"] = responseBody;
        }

        /// <summary>
        /// 完成本次追踪
        /// </summary>
        public void End()
        {
            if (_linkTrackDetail != null) _linkTrackDetail.EndTs = DateTime.Now.ToTimestamps();
            if (_linkTrackContext != null)
            {
                _linkTrackContext.EndTs = DateTime.Now.ToTimestamps();
                if (!Env.IsPro && _linkTrackContext.ContentType != "MessageQueue")
                {
                    var lst = new List<string>
                    {
                        $"Path：{_linkTrackContext.Path}",
                        $"Method：{_linkTrackContext.Method}",
                        $"ContentType：{_linkTrackContext.ContentType}",
                        $"Time：{_linkTrackContext.UseTs} ms"
                    };
                    if (!string.IsNullOrWhiteSpace(_linkTrackContext.RequestBody)) lst.Add($"Body：{_linkTrackContext.RequestBody}");
                    IocManager.Instance.Logger<FsLinkTrack>().LogInformation($"{lst.ToString("\r\n")}");
                }
            }
        }

        /// <summary>
        /// 完成本次追踪
        /// </summary>
        public void Dispose()
        {
            End();
        }

        public void Exception(System.Exception exception)
        {
            if (_linkTrackDetail != null)
            {
                SetHttpResponseBody(exception.ToString());
                _linkTrackDetail.IsException      = true;
                _linkTrackDetail.ExceptionMessage = exception.ToString();
            }

            End();
        }
    }
}