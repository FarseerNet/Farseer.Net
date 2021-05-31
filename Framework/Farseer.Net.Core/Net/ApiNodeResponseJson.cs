// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-02-04 16:38
// ********************************************

using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FS.Core.Net
{
    /// <summary>
    ///     API请求返回结果
    /// </summary>
    [DataContract]
    public class ApiNodeResponseJson
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
        ///     操作是否成功
        /// </summary>
        [DataMember]
        public bool Status { get; set; }

        /// <summary>
        ///     返回值
        /// </summary>
        [DataMember]
        public string ResultValue { get; set; }

        /// <summary>
        ///     返回状态代码
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        ///     返回消息内容
        /// </summary>
        [DataMember]
        public string StatusMessage { get; set; }

        /// <summary>
        /// 服务版本
        /// </summary>
        [DataMember]
        public string ServiceVer { get; set; }
        /// <summary>
        ///     不同接口返回的值
        /// </summary>
        [DataMember]
        public dynamic Data { get; private set; }

        /// <summary>
        /// 设置Data字段的值
        /// </summary>
        /// <param name="data"></param>
        public void SetData(dynamic data)
        {
            Data = data;
        }
        

        /// <summary>
        ///     接口调用成功后返回的Json
        /// </summary>
        /// <param name="statusMessage">成功提示内容</param>
        /// <param name="returnVal">成功返回值</param>
        /// <param name="data">返回的数据列表</param>
        public static ApiNodeResponseJson Success(string statusMessage, object returnVal = null, dynamic data = null)
        {
            return new ApiNodeResponseJson
            {
                Status = true,
                StatusMessage = statusMessage,
                StatusCode = 200,
                ResultValue = returnVal?.ToString(),
                Data = data,
                //SystemName = RegisterInfomation.Register?.SystemName,
                //NodeName = RegisterInfomation.Register?.NodeName
            };
        }

        /// <summary>
        ///     接口调用成功后返回的Json
        /// </summary>
        /// <param name="statusMessage">成功提示内容</param>
        /// <param name="returnVal">成功返回值</param>
        /// <param name="data">返回的数据列表</param>
        public static Task<ApiNodeResponseJson> SuccessAsync(string statusMessage, object returnVal = null, dynamic data = null) => Task.FromResult(Success(statusMessage, returnVal, data));

        /// <summary>
        ///     接口调用失时返回的Json
        /// </summary>
        /// <param name="statusMessage">失败提示内容</param>
        /// <param name="returnVal">失败返回值</param>
        /// <param name="statusCode">失败返回的状态码</param>
        /// <param name="systemName">服务名称</param>
        /// <param name="nodeName">节点名称</param>
        public static ApiNodeResponseJson Error(string statusMessage, object returnVal = null, int statusCode = 403, string systemName = null, string nodeName = null)
        {
            return new ApiNodeResponseJson
            {
                Status = false,
                StatusMessage = statusMessage,
                StatusCode = statusCode,
                ResultValue = returnVal?.ToString(),
                //SystemName = systemName ?? RegisterInfomation.Register?.SystemName,
                //NodeName = nodeName ?? RegisterInfomation.Register?.NodeName
            };
        }

        /// <summary>
        ///     接口调用失时返回的Json
        /// </summary>
        /// <param name="statusMessage">失败提示内容</param>
        /// <param name="returnVal">失败返回值</param>
        /// <param name="statusCode">失败返回的状态码</param>
        /// <param name="systemName">服务名称</param>
        /// <param name="nodeName">节点名称</param>
        public static Task<ApiNodeResponseJson> ErrorAsync(string statusMessage, object returnVal = null, int statusCode = 403, string systemName = null, string nodeName = null) => Task.FromResult(Error(statusMessage, returnVal, statusCode, systemName, nodeName));
    }
}