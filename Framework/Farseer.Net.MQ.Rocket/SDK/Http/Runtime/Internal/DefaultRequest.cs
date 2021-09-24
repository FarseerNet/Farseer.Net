using System;
using System.Collections.Generic;
using System.IO;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal
{
    internal class DefaultRequest : IRequest
    {
        private Stream contentStream;

        public DefaultRequest(WebServiceRequest request, string serviceName)
        {
            if (request == null) throw new ArgumentNullException(paramName: "request");
            if (string.IsNullOrEmpty(value: serviceName)) throw new ArgumentNullException(paramName: "serviceName");

            ServiceName     = serviceName;
            OriginalRequest = request;
            RequestName     = OriginalRequest.GetType().Name;

            foreach (var header in request.Headers) Headers.Add(key: header.Key, value: header.Value);
            foreach (var param in request.Parameters) Parameters.Add(key: param.Key, value: param.Value);
        }

        public string RequestName { get; }

        public string HttpMethod { get; set; } = "GET";

        public bool UseQueryString { get; set; } = false;

        public WebServiceRequest OriginalRequest { get; }

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);


        public IDictionary<string, string> Parameters { get; } = new Dictionary<string, string>(comparer: StringComparer.Ordinal);

        public IDictionary<string, string> SubResources { get; } = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);

        public void AddSubResource(string subResource)
        {
            AddSubResource(subResource: subResource, value: null);
        }

        public void AddSubResource(string subResource, string value)
        {
            SubResources.Add(key: subResource, value: value);
        }

        public Uri Endpoint { get; set; }

        public string ResourcePath { get; set; }

        public byte[] Content { get; set; }

        public Stream ContentStream
        {
            get => contentStream;
            set
            {
                contentStream          = value;
                OriginalStreamPosition = -1;
                if (contentStream != null && contentStream.CanSeek) OriginalStreamPosition = contentStream.Position;
            }
        }

        public long OriginalStreamPosition { get; set; }

        public string ServiceName { get; }

        public bool Suppress404Exceptions { get; set; }

        public bool IsRequestStreamRewindable()
        {
            // Retries may not be possible with a stream
            if (ContentStream != null)
            {
                // Retry is possible if stream is seekable
                return ContentStream.CanSeek;
            }

            return true;
        }

        public bool MayContainRequestBody() => !UseQueryString &&
                                               (HttpMethod == "POST" ||
                                                HttpMethod == "PUT"  ||
                                                HttpMethod == "DELETE");

        public bool HasRequestBody() => (HttpMethod == "POST" ||
                                         HttpMethod == "PUT"  ||
                                         HttpMethod == "DELETE") &&
                                        (Content       != null ||
                                         ContentStream != null);
    }
}