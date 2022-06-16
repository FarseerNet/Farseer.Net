using System;
using System.IO;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Core.Exception;
using FS.Core.Net;
using FS.DI;
using FS.Extends;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Farseer.Net.AspNetCore.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        string requestContent = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(value: httpContext.Request.ContentType))
            {
                // 允许其他管道重复读流
                httpContext.Request.EnableBuffering();
                requestContent = await new StreamReader(stream: httpContext.Request.Body).ReadToEndAsync();
                // 将流定位到初始位置，以让mvc能读到完整的入参
                httpContext.Request.Body.Seek(offset: 0, origin: SeekOrigin.Begin);
            }

            await _next.Invoke(context: httpContext);
        }
        catch (RefuseException refuseException)
        {
            await HandleExceptionAsync(context: httpContext, message: refuseException.Message, statusCode: refuseException.StatusCode);
        }
        catch (Exception e)
        {
            var http = httpContext.Request.IsHttps ? "s" : "";
            using var lst = new PooledList<string>
            {
                $"Path：http{http}://{httpContext.Request.Host}{httpContext.Request.Path}",
                $"Method：{httpContext.Request.Method}",
                $"ContentType：{httpContext.Request.ContentType}",
                $"Body：{requestContent}"
            };

            switch (e)
            {
                case RefuseException re:
                {
                    await HandleExceptionAsync(context: httpContext, message: re.Message, statusCode: re.StatusCode);
                    break;
                }
                case
                {
                }:
                {
                    IocManager.Instance.Logger<ExceptionMiddleware>().LogError(exception: e, message: $"{lst.ToString(sign: "\r\n")}\r\n{e}");
                    await HandleExceptionAsync(context: httpContext, message: "服务器开小差了", statusCode: 500);
                    break;
                }
            }
        }
    }

    private Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
    {
        context.Response.StatusCode  = statusCode;
        context.Response.ContentType = "application/json";
        var apiResponseJson = ApiResponseJson.Error(statusMessage: message, statusCode: statusCode);
        return context.Response.WriteAsync(text: JsonConvert.SerializeObject(value: apiResponseJson));
    }
}