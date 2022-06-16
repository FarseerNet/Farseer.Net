using System;

namespace Farseer.Net.AspNetCore.Attribute;

[AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = true)]
public class ApiResponseAttribute : System.Attribute
{
    public ApiResponseAttribute(int statusCode, string message = "成功")
    {
        StatusCode   = statusCode;
        Message = message;
    }
    
    /// <summary>
    /// 状态码
    /// </summary>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// 消息提示
    /// </summary>
    public string Message { get; set; }
}