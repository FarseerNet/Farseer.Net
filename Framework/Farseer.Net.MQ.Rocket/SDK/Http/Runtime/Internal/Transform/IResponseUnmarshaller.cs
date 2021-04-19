using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    public interface IResponseUnmarshaller<T, R> : IUnmarshaller<T, R>
    {
        AliyunServiceException UnmarshallException(R input, Exception innerException, HttpStatusCode statusCode);
    }
}
