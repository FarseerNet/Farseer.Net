using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    public class HttpWebRequestResponseData : IWebResponseData
    {
        private          string[]            _headerNames;
        private          HashSet<string>     _headerNamesSet;
        private readonly HttpWebResponse     _response;
        private readonly HttpWebResponseBody _responseBody;

        public HttpWebRequestResponseData(HttpWebResponse response)
        {
            _response     = response;
            _responseBody = new HttpWebResponseBody(response: response);

            StatusCode          = response.StatusCode;
            IsSuccessStatusCode = StatusCode >= HttpStatusCode.OK && StatusCode <= (HttpStatusCode)299;
            ContentType         = response.ContentType;
            ContentLength       = response.ContentLength;
        }

        public HttpStatusCode StatusCode { get; }

        public bool IsSuccessStatusCode { get; }

        public string ContentType { get; }

        public long ContentLength { get; }

        public bool IsHeaderPresent(string headerName)
        {
            if (_headerNamesSet == null) SetHeaderNames();
            return _headerNamesSet.Contains(item: headerName);
        }

        public string[] GetHeaderNames()
        {
            if (_headerNames == null) SetHeaderNames();
            return _headerNames;
        }

        public string GetHeaderValue(string name) => _response.GetResponseHeader(headerName: name);

        public IHttpResponseBody ResponseBody => _responseBody;

        private void SetHeaderNames()
        {
            var keys = _response.Headers.Keys;
            _headerNames = new string[keys.Count];
            for (var i = 0; i < keys.Count; i++) _headerNames[i] = keys[index: i];
            _headerNamesSet = new HashSet<string>(collection: _headerNames, comparer: StringComparer.OrdinalIgnoreCase);
        }
    }

    public class HttpWebResponseBody : IHttpResponseBody
    {
        private          bool            _disposed;
        private readonly HttpWebResponse _response;

        public HttpWebResponseBody(HttpWebResponse response)
        {
            _response = response;
        }

        public Stream OpenResponse()
        {
            if (_disposed) throw new ObjectDisposedException(objectName: "HttpWebResponseBody");

            return _response.GetResponseStream();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_response != null) _response.Close();

                _disposed = true;
            }
        }
    }
}