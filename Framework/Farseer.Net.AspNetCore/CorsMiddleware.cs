using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Farseer.Net.AspNetCore;

public class CorsMiddleware
{
    private readonly RequestDelegate _next;

    public CorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.Headers.Add(key: "Access-Control-Allow-Headers", value: httpContext.Request.Headers[key: "Access-Control-Request-Headers"]);
        httpContext.Response.Headers.Add(key: "Access-Control-Allow-Methods", value: httpContext.Request.Headers[key: "Access-Control-Request-Method"]);
        httpContext.Response.Headers.Add(key: "Access-Control-Allow-Credentials", value: "true");
        httpContext.Response.Headers.Add(key: "Access-Control-Max-Age", value: "86400"); //缓存一天
        if (httpContext.Request.Headers[key: "Origin"] != "") httpContext.Response.Headers.Add(key: "Access-Control-Allow-Origin", value: httpContext.Request.Headers[key: "Origin"]);

        if (httpContext.Request.Method == "OPTIONS")
        {
            httpContext.Response.StatusCode = 204;
            await Task.FromResult(result: 0);
            return;
        }

        await _next.Invoke(context: httpContext);
    }
}