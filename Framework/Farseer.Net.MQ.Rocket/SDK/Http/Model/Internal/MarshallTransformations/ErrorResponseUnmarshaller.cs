using System.Xml;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations
{
    /// <summary>
    ///     Response Unmarshaller for all Errors
    /// </summary>
    internal class ErrorResponseUnmarshaller : IUnmarshaller<ErrorResponse, XmlUnmarshallerContext>
    {
        public static ErrorResponseUnmarshaller Instance { get; } = new ErrorResponseUnmarshaller();

        /// <summary>
        ///     Build an ErrorResponse from XML
        /// </summary>
        public ErrorResponse Unmarshall(XmlUnmarshallerContext context)
        {
            var reader = new XmlTextReader(input: context.ResponseStream);

            var errorResponse = new ErrorResponse();
            while (reader.Read())
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.LocalName == Constants.XML_ELEMENT_CODE)
                        {
                            reader.Read();
                            errorResponse.Code = reader.Value;
                        }

                        if (reader.LocalName == Constants.XML_ELEMENT_MESSAGE)
                        {
                            reader.Read();
                            errorResponse.Message = reader.Value;
                        }

                        if (reader.LocalName == Constants.XML_ELEMENT_REQUEST_ID)
                        {
                            reader.Read();
                            errorResponse.RequestId = reader.Value;
                        }

                        if (reader.LocalName == Constants.XML_ELEMENT_HOST_ID)
                        {
                            reader.Read();
                            errorResponse.HostId = reader.Value;
                        }

                        break;
                }

            reader.Close();
            return errorResponse;
        }

        public ErrorResponse Unmarshall(XmlTextReader reader)
        {
            var errorResponse = new ErrorResponse();
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.LocalName == Constants.XML_ELEMENT_CODE)
                        {
                            reader.Read();
                            errorResponse.Code = reader.Value;
                        }

                        if (reader.LocalName == Constants.XML_ELEMENT_MESSAGE)
                        {
                            reader.Read();
                            errorResponse.Message = reader.Value;
                        }

                        if (reader.LocalName == Constants.XML_ELEMENT_REQUEST_ID)
                        {
                            reader.Read();
                            errorResponse.RequestId = reader.Value;
                        }

                        if (reader.LocalName == Constants.XML_ELEMENT_HOST_ID)
                        {
                            reader.Read();
                            errorResponse.HostId = reader.Value;
                        }

                        break;
                }
            }
            while (reader.Read());

            reader.Close();
            return errorResponse;
        }
    }
}