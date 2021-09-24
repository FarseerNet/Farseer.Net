using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    public class AckMessageException : AliyunServiceException
    {
        public AckMessageException() : base(message: exp.ErrorCode.AckMessageFail)
        {
            ErrorCode = exp.ErrorCode.AckMessageFail;
        }

        public List<AckMessageErrorItem> ErrorItems { get; set; } = new List<AckMessageErrorItem>();
    }

    public class AckMessageErrorItem
    {
        public AckMessageErrorItem()
        {
        }

        public AckMessageErrorItem(string errorCode, string errorMessage, string receiptHandle)
        {
            ErrorCode     = errorCode;
            ErrorMessage  = errorMessage;
            ReceiptHandle = receiptHandle;
        }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ReceiptHandle { get; set; }

        public override string ToString() => string.Format(format: "ErrorCode is {0}, ErrorMessage is {1}, ReceiptHandle is {2}",
                                                           arg0: ErrorCode, arg1: ErrorMessage, arg2: ReceiptHandle);
    }
}