using System;
using System.IO;
using System.Xml.Serialization;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    internal class XmlMarshaller<TRequest> : IMarshaller<Stream, TRequest>
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(type: typeof(TRequest));

        public Stream Marshall(TRequest requestObject)
        {
            MemoryStream stream       = null;
            var          gotException = false;
            try
            {
                stream = new MemoryStream();
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(prefix: string.Empty, ns: Constants.MQ_XML_NAMESPACE);
                _serializer.Serialize(stream: stream, o: requestObject, namespaces: namespaces);
                stream.Seek(offset: 0, loc: SeekOrigin.Begin);
            }
            catch (InvalidOperationException ex)
            {
                gotException = true;
                throw new RequestMarshallException(message: ex.Message, innerException: ex);
            }
            finally
            {
                if (gotException && stream != null) stream.Close();
            }

            return stream;
        }
    }
}