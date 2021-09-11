// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-02-04 16:38
// ********************************************

using System.Buffers.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FS.Core.Net
{
    /// <summary>
    ///     API请求返回结果
    /// </summary>
    [DataContract]
    public class ApiResponseJson<TData>
    {
        /// <summary>
        ///     操作是否成功
        /// </summary>
        [DataMember] public bool Status { get; set; }

        /// <summary>
        ///     返回状态代码
        /// </summary>
        [DataMember] public int StatusCode { get; set; }

        /// <summary>
        ///     返回消息内容
        /// </summary>
        [DataMember] public string StatusMessage { get; set; }

        /// <summary>
        ///     不同接口返回的值
        /// </summary>
        [DataMember] public TData Data { get; internal set; }

        /// <summary>
        /// 设置Data字段的值
        /// </summary>
        public void SetData(TData data)
        {
            Data = data;
        }

        /// <summary>
        ///     接口调用成功后返回的Json
        /// </summary>
        /// <param name="statusMessage">成功提示内容</param>
        /// <param name="data">返回的数据列表</param>
        public static ApiResponseJson<TData> Success(string statusMessage, TData data = default)
        {
            return new ApiResponseJson<TData>
            {
                Status        = true,
                StatusMessage = statusMessage,
                StatusCode    = 200,
                Data          = data,
            };
        }

        /// <summary>
        ///     接口调用成功后返回的Json
        /// </summary>
        /// <param name="statusMessage">成功提示内容</param>
        /// <param name="data">返回的数据列表</param>
        public static Task<ApiResponseJson<TData>> SuccessAsync(string statusMessage, TData data = default) => Task.FromResult(Success(statusMessage, data));

        /// <summary>
        ///     接口调用失时返回的Json
        /// </summary>
        /// <param name="statusMessage">失败提示内容</param>
        /// <param name="statusCode">失败返回的状态码</param>
        public static ApiResponseJson<TData> Error(string statusMessage, int statusCode = 403)
        {
            return new ApiResponseJson<TData>
            {
                Status        = false,
                StatusMessage = statusMessage,
                StatusCode    = statusCode,
            };
        }

        /// <summary>
        ///     接口调用失时返回的Json
        /// </summary>
        /// <param name="statusMessage">失败提示内容</param>
        /// <param name="statusCode">失败返回的状态码</param>
        public static Task<ApiResponseJson<TData>> ErrorAsync(string statusMessage, int statusCode = 403) => Task.FromResult(Error(statusMessage, statusCode));

        public static implicit operator ApiResponseJson<TData>(ApiResponseJson api)
        {
            return new ApiResponseJson<TData>
            {
                Status        = api.Status,
                StatusCode    = api.StatusCode,
                StatusMessage = api.StatusMessage,
                Data          = api.Data != null ? Jsons.ToObject<TData>(api.Data) : null
            };
        }

        //public static implicit operator ApiResponseJson(ApiResponseJson<TData> api)
        //{
        //    return new ApiResponseJson
        //    {
        //        Status        = api.Status,
        //        StatusCode    = api.StatusCode,
        //        StatusMessage = api.StatusMessage,
        //        Data          = api.Data
        //    };
        //}
    }
}