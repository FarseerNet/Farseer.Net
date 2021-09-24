using System;
using System.Collections.Generic;
using System.IO;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal
{
    public interface IRequest
    {
        string RequestName { get; }

        IDictionary<string, string> Headers { get; }

        bool UseQueryString { get; set; }

        IDictionary<string, string> Parameters { get; }

        IDictionary<string, string> SubResources { get; }

        string HttpMethod { get; set; }

        Uri Endpoint { get; set; }

        string ResourcePath { get; set; }

        byte[] Content { get; set; }

        Stream ContentStream { get; set; }

        long OriginalStreamPosition { get; set; }

        string ServiceName { get; }

        WebServiceRequest OriginalRequest { get; }

        bool Suppress404Exceptions { get; set; }

        void AddSubResource(string subResource);

        void AddSubResource(string subResource, string value);

        bool IsRequestStreamRewindable();

        bool MayContainRequestBody();

        bool HasRequestBody();
    }
}