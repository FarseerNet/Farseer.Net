// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-06-21 10:20
// ********************************************

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using FS.Core.Fabric;
using FS.Core.Net.Gateway;

namespace FS.Core.Exception
{
    /// <summary>
    /// 用指定的错误消息初始化，并指定错误码
    /// </summary>
    public class RefuseException : System.Exception
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        [DataMember]
        public string SystemName { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        [DataMember]
        public string NodeName { get; set; }

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
        /// 用指定的错误消息初始化，并指定错误码
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="returnVal">返回值</param>
        /// <param name="statusCode">错误状态码</param>
        public RefuseException(string message, object returnVal = null, int statusCode = 403) : base(message)
        {
            StatusCode = statusCode;
            ResultValue = returnVal?.ToString() ?? "";
            SystemName = RegisterInfomation.Register?.SystemName;
            NodeName = RegisterInfomation.Register?.NodeName;
        }

        /// <summary>
        /// 用指定的错误消息初始化，并指定错误码
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="returnVal">返回值</param>
        /// <param name="statusCode">错误状态码</param>
        public static FaultException Throw(string message, object returnVal = null, int statusCode = 403)
        {
            return new FaultException(new FaultReason(message), new FaultCode(statusCode.ToString()), returnVal?.ToString());
        }
    }
}