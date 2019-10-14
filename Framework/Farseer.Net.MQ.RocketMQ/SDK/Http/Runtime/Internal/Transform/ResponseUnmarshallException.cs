﻿using System;
using System.Runtime.Serialization;

namespace FS.MQ.RocketMQ.SDK.Http.Runtime.Internal.Transform
{
    [Serializable]
    internal class ResponseUnmarshallException : InvalidOperationException, ISerializable
    {
        public ResponseUnmarshallException()
        {
        }

         public ResponseUnmarshallException(string message) 
             : base(message)
        {
        }

        public ResponseUnmarshallException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected ResponseUnmarshallException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}