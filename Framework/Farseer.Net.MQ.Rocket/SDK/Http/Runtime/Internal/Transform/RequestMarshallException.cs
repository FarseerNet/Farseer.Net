using System;
using System.Runtime.Serialization;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    internal class RequestMarshallException : InvalidOperationException, ISerializable
    {
        public RequestMarshallException(string message)
            : base(message: message)
        {
        }

        public RequestMarshallException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        protected RequestMarshallException(SerializationInfo info, StreamingContext context)
            : base(info: info, context: context)
        {
        }
    }
}