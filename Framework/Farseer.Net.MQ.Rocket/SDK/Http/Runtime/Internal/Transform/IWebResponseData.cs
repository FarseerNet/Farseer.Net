﻿using System;
using System.IO;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    public interface IWebResponseData
    {
        long           ContentLength       { get; }
        string         ContentType         { get; }
        HttpStatusCode StatusCode          { get; }
        bool           IsSuccessStatusCode { get; }

        IHttpResponseBody ResponseBody { get; }
        string[]          GetHeaderNames();
        bool              IsHeaderPresent(string headerName);
        string            GetHeaderValue(string  headerName);
    }

    public interface IHttpResponseBody : IDisposable
    {
        Stream OpenResponse();
    }
}