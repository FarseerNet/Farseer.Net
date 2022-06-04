namespace FS.Core.Http;

public class HttpResponseResult
{
    public HttpResponseResult(int statusCode, string result)
    {
        HttpCode = statusCode;
        Response = result;
    }

    /// <summary>
    /// Http响应代码
    /// </summary>
    public int HttpCode { get; set; }

    /// <summary>
    /// Http响应结果
    /// </summary>
    public string Response { get; set; }
}