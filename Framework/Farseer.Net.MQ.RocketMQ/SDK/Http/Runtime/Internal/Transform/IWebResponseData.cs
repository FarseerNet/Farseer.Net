using System;
using System.IO;
using System.Net;

namespace FS.MQ.RocketMQ.SDK.Http.Runtime.Internal.Transform
{
    public interface IWebResponseData
    {        
        long ContentLength { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        bool IsSuccessStatusCode { get; }
        string[] GetHeaderNames();
        bool IsHeaderPresent(string headerName);
        string GetHeaderValue(string headerName);

        IHttpResponseBody ResponseBody { get; }
    }

    public interface IHttpResponseBody : IDisposable
    {
        Stream OpenResponse();
    }
}
