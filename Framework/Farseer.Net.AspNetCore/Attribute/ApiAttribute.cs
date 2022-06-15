using System;

namespace Farseer.Net.AspNetCore.Attribute;

[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = true)]
public class ApiAttribute : System.Attribute
{
    public string     RouteUrl   { get; set; }
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// 动态WebApi
    /// </summary>
    /// <param name="routeUrl">路由地址</param>
    /// <param name="httpMethod">Get/Post/Put/Delete </param>
    public ApiAttribute(string routeUrl, HttpMethod httpMethod = HttpMethod.GET)
    {
        RouteUrl   = routeUrl.StartsWith("/") ? routeUrl.Substring(1) : routeUrl;
        HttpMethod = httpMethod;
    }
}

public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE,
}