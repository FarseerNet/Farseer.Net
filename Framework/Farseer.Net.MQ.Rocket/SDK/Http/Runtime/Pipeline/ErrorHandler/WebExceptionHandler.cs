using System.Globalization;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.ErrorHandler
{
    public class WebExceptionHandler : ExceptionHandler<WebException>
    {
        public override bool HandleException(IExecutionContext executionContext, WebException exception)
        {
            var requestContext    = executionContext.RequestContext;
            var httpErrorResponse = exception.Response as HttpWebResponse;

            var message = string.Format(provider: CultureInfo.InvariantCulture,
                                        format: "A WebException with status {0} was thrown, caused by {1}", arg0: exception.Status, arg1: exception.Message);
            throw new AliyunServiceException(message: message, innerException: exception);
        }
    }
}