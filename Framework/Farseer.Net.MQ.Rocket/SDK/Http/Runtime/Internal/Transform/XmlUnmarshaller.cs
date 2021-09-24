using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    internal class XmlUnmarshaller<TResponse> : IUnmarshaller<TResponse, Stream>
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(type: typeof(TResponse));

        public TResponse Unmarshall(Stream responseStream) => Unmarshall(responseStream: responseStream, keepOpenOnException: false);

        public TResponse Unmarshall(Stream responseStream, bool keepOpenOnException = false)
        {
            var dispose = true;
            try
            {
                return (TResponse)_serializer.Deserialize(stream: responseStream);
            }
            catch (XmlException ex)
            {
                if (keepOpenOnException) dispose = false;
                throw new ResponseUnmarshallException(message: ex.Message, innerException: ex);
            }
            catch (InvalidOperationException ex)
            {
                if (keepOpenOnException) dispose = false;
                throw new ResponseUnmarshallException(message: ex.Message, innerException: ex);
            }
            finally
            {
                if (dispose)
                    responseStream.Dispose();
                else
                    responseStream.Seek(offset: 0, origin: SeekOrigin.Begin);
            }
        }
    }
}