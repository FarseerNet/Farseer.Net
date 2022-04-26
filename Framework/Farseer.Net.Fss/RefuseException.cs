// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-06-21 10:20
// ********************************************

using System.Runtime.Serialization;
using System.ServiceModel;

namespace FS.Fss
{
    /// <summary>
    ///     用指定的错误消息初始化，并指定错误码
    /// </summary>
    internal class FssException : System.Exception
    {
        /// <summary>
        ///     用指定的错误消息初始化，并指定错误码
        /// </summary>
        /// <param name="message"> 提示消息 </param>
        /// <param name="returnVal"> 返回值 </param>
        /// <param name="statusCode"> 错误状态码 </param>
        public FssException(string message, object returnVal = null, int statusCode = 403) : base(message: message)
        {
            StatusCode  = statusCode;
            ResultValue = returnVal?.ToString() ?? "";
        }

        /// <summary>
        ///     返回状态代码
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        ///     返回值
        /// </summary>
        [DataMember]
        public string ResultValue { get; set; }

        /// <summary>
        ///     用指定的错误消息初始化，并指定错误码
        /// </summary>
        /// <param name="message"> 提示消息 </param>
        /// <param name="returnVal"> 返回值 </param>
        /// <param name="statusCode"> 错误状态码 </param>
        public static FaultException Throw(string message, object returnVal = null, int statusCode = 403) => new(reason: new FaultReason(text: message), code: new FaultCode(name: statusCode.ToString()), action: returnVal?.ToString());
    }
}