using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FS.Core.Exception;
using FS.Core.Net;
using FS.DI;
using FS.Extends;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FS
{
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
                if (!string.IsNullOrWhiteSpace(httpContext.Request.ContentType))
                {
                    // 允许其他管道重复读流
                    httpContext.Request.EnableBuffering();
                    requestContent = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                    // 将流定位到初始位置，以让mvc能读到完整的入参
                    httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                }

                await _next.Invoke(httpContext);
            }
            catch (Exception e)
            {
                var http = httpContext.Request.IsHttps ? "s" : "";
                var lst = new List<string>
                {
                    $"Path：http{http}://{httpContext.Request.Host}{httpContext.Request.Path}",
                    $"Method：{httpContext.Request.Method}",
                    $"ContentType：{httpContext.Request.ContentType}",
                    $"Body：{requestContent}",
                };

                switch (e)
                {
                    case RefuseException re:
                    {
                        await HandleExceptionAsync(httpContext, re.Message, re.StatusCode);
                        break;
                    }
                    case { }:
                    {
                        IocManager.Instance.Logger.Error($"{lst.ToString("\r\n")}\r\n{e.ToString()}");
                        await HandleExceptionAsync(httpContext, "服务器开小差了", 500);
                        break;
                    }
                }
            }
        }

        private Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            context.Response.StatusCode  = statusCode;
            context.Response.ContentType = "application/json";
            var apiResponseJson = ApiResponseJson.Error(message, statusCode);
            return context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponseJson));
        }
    }
}