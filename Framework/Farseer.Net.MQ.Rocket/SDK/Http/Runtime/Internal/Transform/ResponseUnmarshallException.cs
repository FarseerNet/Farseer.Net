using System;
using System.Runtime.Serialization;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    [Serializable]
    internal class ResponseUnmarshallException : InvalidOperationException, ISerializable
    {
        public ResponseUnmarshallException()
        {
        }

        public ResponseUnmarshallException(string message)
            : base(message: message)
        {
        }

        public ResponseUnmarshallException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        protected ResponseUnmarshallException(SerializationInfo info, StreamingContext context)
            : base(info: info, context: context)
        {
        }
    }
}