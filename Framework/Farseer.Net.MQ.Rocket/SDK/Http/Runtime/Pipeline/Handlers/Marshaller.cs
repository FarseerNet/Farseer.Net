using System.Globalization;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.Handlers
{
    public class Marshaller : GenericHandler
    {
        protected override void PreInvoke(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            requestContext.Request          = requestContext.Marshaller.Marshall(input: requestContext.OriginalRequest);
            requestContext.Request.Endpoint = requestContext.ClientConfig.RegionEndpoint;
            AddRequiredHeaders(requestContext: requestContext);
            AddOptionalHeaders(requestContext: requestContext);
        }

        private void AddRequiredHeaders(IRequestContext requestContext)
        {
            var headers = requestContext.Request.Headers;
            headers[key: Constants.UserAgentHeader] = requestContext.ClientConfig.UserAgent;
            if (requestContext.Request.ContentStream != null) headers[key: Constants.ContentLengthHeader] = requestContext.Request.ContentStream.Length.ToString(provider: CultureInfo.InvariantCulture);
            headers[key: Constants.DateHeader]       = AliyunSDKUtils.FormattedCurrentTimestampRFC822;
            headers[key: Constants.XMQVersionHeader] = requestContext.ClientConfig.ServiceVersion;
            if (!headers.ContainsKey(key: Constants.HostHeader))
            {
                var requestEndpoint                            = requestContext.Request.Endpoint;
                var hostHeader                                 = requestEndpoint.Host;
                if (!requestEndpoint.IsDefaultPort) hostHeader += ":" + requestEndpoint.Port;
                headers.Add(key: Constants.HostHeader, value: hostHeader);
            }
        }

        private void AddOptionalHeaders(IRequestContext requestContext)
        {
            var originalRequest = requestContext.Request.OriginalRequest;
            var headers         = requestContext.Request.Headers;
            if (originalRequest.IsSetContentType())
                headers[key: Constants.ContentTypeHeader] = originalRequest.ContentType;
            else
                headers[key: Constants.ContentTypeHeader] = Constants.ContentTypeTextXml;
            if (originalRequest.IsSetContentMD5()) headers[key: Constants.ContentMD5Header] = originalRequest.ContentMD5;
        }
    }
}