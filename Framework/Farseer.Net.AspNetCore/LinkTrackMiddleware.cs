using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
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

        public async Task Invoke(HttpContext httpContext, IIocManager ioc)
        {
            if (string.IsNullOrWhiteSpace(httpContext.Request.ContentType))
            {
                await _next.Invoke(httpContext);
                return;
            }

            httpContext.Request.Headers.TryGetValue("FsContextId", out var contextId);
            if (!string.IsNullOrWhiteSpace(contextId))
            {
                httpContext.Request.Headers.TryGetValue("FsAppId", out var parentAppId);
                FsLinkTrack.Current.Set(contextId, parentAppId);
            }

            // 读取请求入参
            string requestContent = null;
            switch (httpContext.Request.Method)
            {
                case "GET":
                    requestContent = httpContext.Request.QueryString.Value;
                    break;
                default:
                {
                    if (!string.IsNullOrWhiteSpace(httpContext.Request.ContentType))
                    {
                        // 允许其他管道重复读流
                        httpContext.Request.EnableBuffering();
                        requestContent = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                        // 将流定位到初始位置，以让mvc能读到完整的入参
                        httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                    }

                    break;
                }
            }

            // 读取响应Body
            var path      = httpContext.Request.Path.Value?.ToLower();
            var dicHeader = httpContext.Request.Headers.ToDictionary(o => o.Key, o => o.Value.ToString());
            using (var trackEnd = FsLinkTrack.TrackApiServer(httpContext.Request.Host.Host, path, httpContext.Request.Method, httpContext.Request.ContentType, dicHeader, requestContent, httpContext.GetIP()))
            {
                var originalBodyStream = httpContext.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    // 先将MemoryStream给Body，用于后续取响应内容
                    httpContext.Response.Body = responseBody;

                    await _next.Invoke(httpContext);

                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    var rspBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);

                    // 如果是api接口，则记录返回值
                    if (!string.IsNullOrWhiteSpace(httpContext.Response.ContentType) && (httpContext.Response.ContentType.Contains("json") ||
                                                                                         httpContext.Response.ContentType.Contains("xml") ||
                                                                                         httpContext.Response.ContentType.Contains("text")))
                    {
                        trackEnd.SetResponseBody(rspBody);
                    }
                }
            }

            // 写入链路追踪
            LinkTrackQueue.Enqueue();
        }
    }
}