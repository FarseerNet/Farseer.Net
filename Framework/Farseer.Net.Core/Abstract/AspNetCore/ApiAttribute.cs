using System;

namespace FS.Core.Abstract.AspNetCore;

[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = true)]
public class ApiAttribute : System.Attribute
{
    /// <summary>
    /// 路由地址
    /// </summary>
    public string RouteUrl { get; set; }
    /// <summary>
    /// Get/Post/Put/Delete
    /// </summary>
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// api返回的状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// api返回的消息提示
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 动态WebApi
    /// </summary>
    /// <param name="routeUrl">路由地址</param>
    /// <param name="httpMethod">Get/Post/Put/Delete </param>
    /// <param name="statusCode">状态码</param>
    /// <param name="message">消息提示</param>
    public ApiAttribute(string routeUrl, HttpMethod httpMethod = HttpMethod.POST, string message = "成功", int statusCode = 200)
    {
        RouteUrl   = routeUrl.StartsWith("/") ? routeUrl.Substring(1) : routeUrl;
        HttpMethod = httpMethod;
        StatusCode = statusCode;
        Message    = message;
    }
}
public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE,
}