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
    public class AckMessageRequestMarshaller : IMarshaller<IRequest, AckMessageRequest>, IMarshaller<IRequest, WebServiceRequest>
    {
        public static AckMessageRequestMarshaller Instance { get; } = new AckMessageRequestMarshaller();

        public IRequest Marshall(AckMessageRequest publicRequest)
        {
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(w: stream, encoding: Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement(localName: Constants.XML_ROOT_RECEIPT_HANDLES, ns: Constants.MQ_XML_NAMESPACE);
            foreach (var receiptHandle in publicRequest.ReceiptHandles) writer.WriteElementString(localName: Constants.XML_ELEMENT_RECEIPT_HANDLE, value: receiptHandle);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            stream.Seek(offset: 0, loc: SeekOrigin.Begin);

            IRequest request = new DefaultRequest(request: publicRequest, serviceName: Constants.MQ_SERVICE_NAME);
            request.HttpMethod    = HttpMethod.DELETE.ToString();
            request.ContentStream = stream;
            request.ResourcePath = Constants.TOPIC_PRE_RESOURCE + publicRequest.TopicName
                                                                + Constants.MESSAGE_SUB_RESOURCE;
            PopulateSpecialParameters(request: publicRequest, paramters: request.Parameters);
            return request;
        }

        public IRequest Marshall(WebServiceRequest input) => Marshall(publicRequest: (AckMessageRequest)input);

        private static void PopulateSpecialParameters(AckMessageRequest request, IDictionary<string, string> paramters)
        {
            paramters.Add(key: Constants.PARAMETER_CONSUMER, value: request.Consumer);
            if (request.IsSetInstance()) paramters.Add(key: Constants.PARAMETER_NS, value: request.IntanceId);
            if (request.IsSetTransaction()) paramters.Add(key: Constants.PARAMETER_TRANSACTION, value: request.Trasaction);
        }
    }
}