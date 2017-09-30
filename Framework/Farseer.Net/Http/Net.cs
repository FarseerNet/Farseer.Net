// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-09-15 9:32
// ********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Farseer.Net.Context;
using Farseer.Net.Extends;

namespace Farseer.Net.Http
{
    public class Net
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// http request请求
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="postData">查询条件</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static async Task<string> GetAsync(string url, string postData = null, Encoding encoding = null, int requestTimeout = 0, CookieContainer cookie = null)
        {
            if (encoding == null) { encoding = Encoding.UTF8; }
            if (requestTimeout > 0) { HttpClient.Timeout = TimeSpan.FromMilliseconds(requestTimeout); }

            var httpRspMessage = HttpClient.GetAsync(string.IsNullOrWhiteSpace(postData) ? url : $"{url}&{postData}");
            if (requestTimeout > 0) { httpRspMessage.Wait(new CancellationTokenSource(requestTimeout).Token); }

            var bytes = await (await httpRspMessage.ConfigureAwait(false)).Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// http request请求
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="postData">查询条件</param>
        /// <param name="contentType">获取或设置 Content-type HTTP 标头的值。</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static async Task<string> PostAsync(string url, string postData = "", Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            if (encoding == null) { encoding = Encoding.UTF8; }
            if (requestTimeout > 0) { HttpClient.Timeout = TimeSpan.FromMilliseconds(requestTimeout); }

            var httpContent = new StringContent(postData, encoding, contentType);// 内容体
            httpContent.Headers.AddTraceInfoToHeader(); // 添加头部
            //httpContent.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
            var httpRspMessage = HttpClient.PostAsync(url, httpContent);

            if (requestTimeout > 0) { httpRspMessage.Wait(new CancellationTokenSource(requestTimeout).Token); }

            var bytes = await (await httpRspMessage.ConfigureAwait(false)).Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return encoding.GetString(bytes);
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">字典类型</param>
        /// <param name="contentType">获取或设置 Content-type HTTP 标头的值。</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static async Task<string> PostAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null) => await PostAsync(url, postData.Select(keyVal => $"{keyVal.Key}={keyVal.Value}").ToString("&"), encoding, contentType, requestTimeout, cookie);

        /// <summary>
        ///     获取网络IP
        /// </summary>
        public static string GetIP()
        {
            var ip = "127.0.0.1";
            var strHostName = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(strHostName);
            foreach (var item in ipHost.AddressList) { if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) { ip = item.ToString(); } }
            return ip;
        }
    }
}