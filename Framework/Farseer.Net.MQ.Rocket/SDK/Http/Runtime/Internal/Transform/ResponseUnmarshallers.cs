using System;
using System.IO;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    public abstract class ResponseUnmarshaller : IResponseUnmarshaller<WebServiceResponse, UnmarshallerContext>
    {
        internal virtual bool HasStreamingProperty => false;

        #region IResponseUnmarshaller<WebServiceResponse,UnmarshallerContext> Members

        public virtual AliyunServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode) => throw new NotImplementedException();

        #endregion

        #region IUnmarshaller<WebServiceResponse,UnmarshallerContext> Members

        public abstract WebServiceResponse Unmarshall(UnmarshallerContext input);

        #endregion

        public virtual UnmarshallerContext CreateContext(IWebResponseData response, Stream stream)
        {
            if (response == null) throw new WebException(message: "The Web Response for a successful request is null!");

            return ConstructUnmarshallerContext(responseStream: stream, response: response);
        }

        public WebServiceResponse UnmarshallResponse(UnmarshallerContext context)
        {
            var response = Unmarshall(input: context);
            response.ContentLength  = context.ResponseData.ContentLength;
            response.HttpStatusCode = context.ResponseData.StatusCode;
            return response;
        }

        protected abstract UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, IWebResponseData response);
    }

    public abstract class XmlResponseUnmarshaller : ResponseUnmarshaller
    {
        public override WebServiceResponse Unmarshall(UnmarshallerContext input)
        {
            var context = input as XmlUnmarshallerContext;
            if (context == null) throw new InvalidOperationException(message: "Unsupported UnmarshallerContext");

            var response = Unmarshall(input: context);

            foreach (var headerName in context.ResponseData.GetHeaderNames()) response.Headers.Add(key: headerName, value: context.ResponseData.GetHeaderValue(headerName: headerName));

            return response;
        }

        public override AliyunServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode)
        {
            var context = input as XmlUnmarshallerContext;
            if (context == null) throw new InvalidOperationException(message: "Unsupported UnmarshallerContext");

            return UnmarshallException(input: context, innerException: innerException, statusCode: statusCode);
        }

        protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, IWebResponseData response) => new XmlUnmarshallerContext(responseStream: responseStream, responseData: response);

        public abstract WebServiceResponse Unmarshall(XmlUnmarshallerContext input);

        public abstract AliyunServiceException UnmarshallException(XmlUnmarshallerContext input, Exception innerException, HttpStatusCode statusCode);
    }
}