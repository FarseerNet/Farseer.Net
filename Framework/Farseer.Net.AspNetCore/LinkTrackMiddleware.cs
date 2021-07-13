using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.Extends;
using FS.LinkTrack;
using Microsoft.AspNetCore.Http;

namespace FS
{
    /// <summary>
    /// 链路追踪
    /// </summary>
    public class LinkTrackMiddleware
    {
        private readonly RequestDelegate _next;

        public LinkTrackMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,IIocManager ioc)
        {
            if (!string.IsNullOrWhiteSpace(httpContext.Request.ContentType))
            {
                // 允许其他管道重复读流
                httpContext.Request.EnableBuffering();
                httpContext.Request.Headers.TryGetValue("FsContextId", out var contextId);
                if (!string.IsNullOrWhiteSpace(contextId))
                {
                    FsLinkTrack.Current.Set(contextId);
                }
            }

            var path = httpContext.Request.Path.Value?.ToLower();
            FsLinkTrack.Current.Set(new LinkTrackDetail
            {
                CallType          = EumCallType.ApiServer,
                StartTs           = DateTime.Now.ToTimestamps(),
                GrpcLinkTrack = new GrpcLinkTrackDetail()
                {
                    Server = path.Substring(0, path.LastIndexOf('/')),
                    Action = path.Split('/').LastOrDefault()
                }
            });
            
            await _next.Invoke(httpContext);
            
            // 写入链路追踪
            LinkTrackQueue.Enqueue();
        }
    }
}