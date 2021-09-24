using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations
{
    internal class ConsumeMessageRequestMarshaller : IMarshaller<IRequest, ConsumeMessageRequest>, IMarshaller<IRequest, WebServiceRequest>
    {
        public static ConsumeMessageRequestMarshaller Instance { get; } = new ConsumeMessageRequestMarshaller();

        public IRequest Marshall(ConsumeMessageRequest publicRequest)
        {
            IRequest request = new DefaultRequest(request: publicRequest, serviceName: Constants.MQ_SERVICE_NAME);
            request.HttpMethod = HttpMethod.GET.ToString();
            request.ResourcePath = Constants.TOPIC_PRE_RESOURCE + publicRequest.TopicName
                                                                + Constants.MESSAGE_SUB_RESOURCE;
            PopulateSpecialParameters(request: publicRequest, paramters: request.Parameters);
            return request;
        }

        public IRequest Marshall(WebServiceRequest input) => Marshall(publicRequest: (ConsumeMessageRequest)input);

        private static void PopulateSpecialParameters(ConsumeMessageRequest request, IDictionary<string, string> paramters)
        {
            paramters.Add(key: Constants.PARAMETER_CONSUMER, value: request.Consumer);
            if (request.IsSetInstance()) paramters.Add(key: Constants.PARAMETER_NS, value: request.IntanceId);
            if (request.IsSetWaitSeconds() && request.WaitSeconds > 0 && request.WaitSeconds < 31) paramters.Add(key: Constants.PARAMETER_WAIT_SECONDS, value: request.WaitSeconds.ToString());
            paramters.Add(key: Constants.PARAMETER_BATCH_SIZE, value: request.BatchSize.ToString());
            if (request.IsSetMessageTag()) paramters.Add(key: Constants.PARAMETER_CONSUME_TAG, value: request.MessageTag);
            if (request.IsSetTransaction()) paramters.Add(key: Constants.PARAMETER_TRANSACTION, value: request.Trasaction);
        }
    }
}