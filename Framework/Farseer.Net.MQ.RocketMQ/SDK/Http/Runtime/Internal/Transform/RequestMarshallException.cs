﻿using System;
using System.Runtime.Serialization;

namespace FS.MQ.RocketMQ.SDK.Http.Runtime.Internal.Transform
{
    internal class RequestMarshallException : InvalidOperationException, ISerializable
    {
        public RequestMarshallException(string message) 
            : base(message)
        {
        }

        public RequestMarshallException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected RequestMarshallException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
