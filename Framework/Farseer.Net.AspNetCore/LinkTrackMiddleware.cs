using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using Microsoft.AspNetCore.Http;

namespace FS
{
    /// <summary>
    ///     链路追踪（Web Api入口）
    /// </summary>
    public class LinkTrackMiddleware
    {
        private readonly RequestDelegate _next;

        public LinkTrackMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IIocManager _iocManager)
        {
            if (string.IsNullOrWhiteSpace(value: httpContext.Request.ContentType))
            {
                await _next.Invoke(context: httpContext);
                return;
            }

            httpContext.Request.Headers.TryGetValue(key: "FsContextId", value: out var contextId);
            var parentAppId = "";
            if (!string.IsNullOrWhiteSpace(value: contextId))
            {
                httpContext.Request.Headers.TryGetValue(key: "FsAppId", out var parentAppId2);
                parentAppId = parentAppId2.ToString();
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
                    if (!string.IsNullOrWhiteSpace(value: httpContext.Request.ContentType))
                    {
                        // 允许其他管道重复读流
                        httpContext.Request.EnableBuffering();
                        requestContent = await new StreamReader(stream: httpContext.Request.Body).ReadToEndAsync();
                        // 将流定位到初始位置，以让mvc能读到完整的入参
                        httpContext.Request.Body.Seek(offset: 0, origin: SeekOrigin.Begin);
                    }

                    break;
                }
            }

            // 读取响应Body
            var path = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}{httpContext.Request.Path.Value?.ToLower()}";

            var dicHeader = httpContext.Request.Headers.ToDictionary(keySelector: o => o.Key, elementSelector: o => o.Value.ToString());
            using (var trackEnd = FsLinkTrack.TrackApiServer(contextId, parentAppId, domain: httpContext.Request.Host.Host, path: path, method: httpContext.Request.Method, contentType: httpContext.Request.ContentType, headerDictionary: dicHeader, requestBody: requestContent, ip: httpContext.GetIP()))
            {
                var originalBodyStream = httpContext.Response.Body;
                await using (var responseBody = new MemoryStream())
                {
                    // 先将MemoryStream给Body，用于后续取响应内容
                    httpContext.Response.Body = responseBody;

                    try
                    {
                        await _next.Invoke(context: httpContext);

                        httpContext.Response.Body.Seek(offset: 0, origin: SeekOrigin.Begin);
                        var rspBody = await new StreamReader(stream: httpContext.Response.Body).ReadToEndAsync();
                        httpContext.Response.Body.Seek(offset: 0, origin: SeekOrigin.Begin);
                        await responseBody.CopyToAsync(destination: originalBodyStream);

                        // 如果是api接口，则记录返回值
                        if (!string.IsNullOrWhiteSpace(value: httpContext.Response.ContentType) && (httpContext.Response.ContentType.Contains(value: "json") ||
                                                                                                    httpContext.Response.ContentType.Contains(value: "xml")  ||
                                                                                                    httpContext.Response.ContentType.Contains(value: "text")))
                            trackEnd.SetDownstreamResponseBody(responseBody: rspBody);
                    }
                    catch (Exception e)
                    {
                        trackEnd.Exception(exception: e);
                        throw;
                    }
                }
            }
        }
    }
}