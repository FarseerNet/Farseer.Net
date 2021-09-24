using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations
{
    /// <summary>
    ///     PublishMessage Request Marshaller
    /// </summary>
    internal class PublishMessageRequestMarshaller : IMarshaller<IRequest, PublishMessageRequest>, IMarshaller<IRequest, WebServiceRequest>
    {
        public static PublishMessageRequestMarshaller Instance { get; } = new PublishMessageRequestMarshaller();

        public IRequest Marshall(PublishMessageRequest publicRequest)
        {
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(w: stream, encoding: Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement(localName: Constants.XML_ROOT_MESSAGE, ns: Constants.MQ_XML_NAMESPACE);
            if (publicRequest.IsSetMessageBody()) writer.WriteElementString(localName: Constants.XML_ELEMENT_MESSAGE_BODY, value: publicRequest.MessageBody);
            if (publicRequest.IsSetMessageTag()) writer.WriteElementString(localName: Constants.XML_ELEMENT_MESSAGE_TAG, value: publicRequest.MessageTag);
            if (publicRequest.IsSetProperties()) writer.WriteElementString(localName: Constants.XML_ELEMENT_MESSAGE_PROPERTIES, value: publicRequest.Properties);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();

            stream.Seek(offset: 0, loc: SeekOrigin.Begin);

            IRequest request = new DefaultRequest(request: publicRequest, serviceName: Constants.MQ_SERVICE_NAME);

            request.HttpMethod    = HttpMethod.POST.ToString();
            request.ContentStream = stream;
            request.ResourcePath = Constants.TOPIC_PRE_RESOURCE + publicRequest.TopicName
                                                                + Constants.MESSAGE_SUB_RESOURCE;
            PopulateSpecialParameters(request: publicRequest, paramters: request.Parameters);
            return request;
        }

        public IRequest Marshall(WebServiceRequest input) => Marshall(publicRequest: (PublishMessageRequest)input);

        private static void PopulateSpecialParameters(PublishMessageRequest request, IDictionary<string, string> paramters)
        {
            if (request.IsSetInstance()) paramters.Add(key: Constants.PARAMETER_NS, value: request.IntanceId);
        }
    }
}