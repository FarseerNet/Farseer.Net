using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FS
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers",     httpContext.Request.Headers["Access-Control-Request-Headers"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods",     httpContext.Request.Headers["Access-Control-Request-Method"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            httpContext.Response.Headers.Add("Access-Control-Max-Age",           "86400"); //缓存一天
            if (httpContext.Request.Headers["Origin"] != "")
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"]);
            }

            if (httpContext.Request.Method == "OPTIONS")
            {
                httpContext.Response.StatusCode = 204;
                await Task.FromResult(0);
                return;
            }

            await _next.Invoke(httpContext);
        }
    }
}