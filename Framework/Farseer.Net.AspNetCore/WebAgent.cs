using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Farseer.Net.AspNetCore;

public static class WebAgent
{
    /// <summary>
    ///     没有IP地址
    /// </summary>
    private const string NO_IP = "0.0.0.0";

    /// <summary>
    ///     不支持的IP（可能是IPv6）
    /// </summary>
    private const string ERROR_IP = "255.255.255.255";

    /// <summary>
    ///     IP库的路径
    /// </summary>
    private const string IPDATA_PATH = "ipipfree.ipdb";
    private static readonly string[] keys = { "Ali-CDN-Real-IP", "X-Real-IP", "X-Forwarded-IP", "X-Forwarded-For", "X-Original-Forwarded-For" };
    
    /// <summary>
    ///     IPv4的正则验证
    /// </summary>
    public static readonly Regex regex = new(pattern: @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

    public static string GetIP(this HttpContext context)
    {
        if (context == null) return NO_IP;
        var ip = string.Empty;

        foreach (var key in keys)
        {
            if (key == null || !context.Request.Headers.ContainsKey(key: key)) continue;
            string value = context.Request.Headers[key: key];
            if (regex.IsMatch(input: value))
            {
                ip = regex.Match(input: value).Value;
                break;
            }
        }

        if (string.IsNullOrEmpty(value: ip)) ip = context.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.MapToIPv4().ToString();
        if (!regex.IsMatch(input: ip)) ip       = ERROR_IP;
        return ip;
    }
}