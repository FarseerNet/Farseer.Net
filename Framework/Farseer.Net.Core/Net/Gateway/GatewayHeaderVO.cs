// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-02-24 17:10
// ********************************************

using System.Security.Cryptography;
using System.Text;

namespace Farseer.Net.Core.Net.Gateway
{
    /// <summary>
    ///     网关转发过来的头部包
    /// </summary>
    public class GatewayHeaderVO
    {
        /// <summary>
        ///     时间戳
        /// </summary>
        public long GatewayTimestamp { get; set; }

        /// <summary>
        ///     版本号
        /// </summary>
        public string GatewayVer { get; set; }

        /// <summary>
        ///     服务版本号
        /// </summary>
        public string ServiceVer { get; set; }

        /// <summary>
        ///     签名参数
        /// </summary>
        public string ServiceSign { get; set; }

        /// <summary>
        ///     授权应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        ///     客户端版本
        /// </summary>
        public string AppVer { get; set; }

        /// <summary>
        ///     客户端IP
        /// </summary>
        public string AppIP { get; set; }

        /// <summary>
        ///     客户端编号
        /// </summary>
        public string ServiceKey { get; set; }

        /// <summary>
        ///     系统别名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>根 </summary>
        public string RootId { get; set; }

        /// <summary>父</summary>
        public string ParentId { get; set; }

        /// <summary>当前</summary>
        public string ChildId { get; set; }

        /// <summary>跟踪Id</summary>
        public string TraceId { get; set; }
        

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <returns></returns>
        public string BuilderSign() => MD5($"{GatewayTimestamp}@{GatewayVer}@{AppID}@{ServiceKey}@{ServiceName}");

        /// <summary>
        ///     MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">默认编码Encoding.Default</param>
        /// <returns>MD5结果</returns>
        private string MD5(string str, Encoding encoding = null)
        {
            if (encoding == null) { encoding = Encoding.Default; }
            var bytes = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(str));
            var md5 = string.Empty;
            for (var i = 0; i < bytes.Length; i++) { md5 += bytes[i].ToString("x").PadLeft(2, '0'); }

            return md5;
        }
    }
}