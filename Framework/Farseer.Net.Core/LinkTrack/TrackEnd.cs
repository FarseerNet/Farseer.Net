using System;
using System.Collections.Generic;
using Collections.Pooled;
using FS.Core.Http;
using FS.DI;
using FS.Extends;
using Microsoft.Extensions.Logging;

namespace FS.Core.LinkTrack
{
    /// <summary>
    ///     本次操作完成
    /// </summary>
    public class TrackEnd : IDisposable
    {
        private readonly LinkTrackContext _linkTrackContext;
        private readonly LinkTrackDetail  _linkTrackDetail;
        public TrackEnd(LinkTrackContext linkTrackContext)
        {
            _linkTrackContext = linkTrackContext;
        }

        public TrackEnd(LinkTrackDetail linkTrackDetail)
        {
            _linkTrackDetail = linkTrackDetail;
        }

        /// <summary>
        ///     完成本次追踪
        /// </summary>
        public void Dispose()
        {
            End();
        }

        /// <summary>
        ///     设置下游系统API响应内容
        /// </summary>
        public void SetDownstreamResponseBody(string responseBody, int responseStatusCode)
        {
            _linkTrackContext.ResponseBody = responseBody;
            _linkTrackContext.StatusCode   = responseStatusCode.ToString();
        }

        /// <summary>
        ///     设置Http响应内容
        /// </summary>
        public void SetHttpResponseBody(string responseBody, int responseStatusCode)
        {
            _linkTrackDetail.Data[key: "ResponseBody"] = responseBody;
            _linkTrackDetail.Data["StatusCode"]        = responseStatusCode.ToString();
        }

        /// <summary>
        ///     设置Http响应内容
        /// </summary>
        public void SetHttpResponseBody(HttpResponseResult httpResponseResult)
        {
            _linkTrackDetail.Data["ResponseBody"] = httpResponseResult.Response;
            _linkTrackDetail.Data["StatusCode"]   = httpResponseResult.HttpCode.ToString();
        }

        /// <summary>
        ///     完成本次追踪
        /// </summary>
        public void End()
        {
            if (_linkTrackDetail != null) _linkTrackDetail.EndTs = DateTime.Now.ToTimestamps();
            if (_linkTrackContext != null)
            {
                _linkTrackContext.EndTs = DateTime.Now.ToTimestamps();
                if (!Env.IsPro)
                {
                    using var lst = new PooledList<string>
                    {
                        $"{_linkTrackContext.ContentType} {_linkTrackContext.Method}：{_linkTrackContext.Path} {_linkTrackContext.UseTs} ms",
                    };
                    if (!string.IsNullOrWhiteSpace(value: _linkTrackContext.RequestBody)) lst.Add(item: $"Body：{_linkTrackContext.RequestBody}");
                    IocManager.Instance.Logger<FsLinkTrack>().LogInformation(message: $"{lst.ToString(sign: "\r\n")}");
                }
                if (FsLinkTrack.IsUseLinkTrack) IocManager.GetService<ILinkTrackQueue>().Enqueue();
            }
        }

        public void Exception(System.Exception exception)
        {
            if (_linkTrackDetail != null)
            {
                SetHttpResponseBody(responseBody: exception.ToString(), 500);
                _linkTrackDetail.IsException      = true;
                _linkTrackDetail.ExceptionMessage = exception.ToString();
            }

            End();
        }
    }
}