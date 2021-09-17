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
using FS.Core.LinkTrack;
using FS.Extends;
using Newtonsoft.Json;

namespace FS.Core.Http
{
    public class Net
    {
        private static readonly HttpClient httpClient = new(new HttpClientHandler()
        {
            UseProxy                                  = true,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });

        static Net()
        {
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            //httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        /// <summary>
        /// http request请求
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="postData">查询条件</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static Task<string> GetAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, int requestTimeout = 0, CookieContainer cookie = null) => GetAsync(url, postData.Select(keyVal => $"{keyVal.Key}={keyVal.Value}").ToString("&"), null, encoding, requestTimeout, cookie);

        /// <summary>
        /// http request请求
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="postData">查询条件</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="headerData">添加头部信息 </param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static async Task<string> GetAsync(string url, string postData = null, Dictionary<string, string> headerData = null, Encoding encoding = null, int requestTimeout = 0, CookieContainer cookie = null)
        {
            using (var trackEnd = FsLinkTrack.TrackHttp(url, "GET", headerData, postData))
            {
                encoding ??= Encoding.UTF8;

                var httpContent = new StringContent("", encoding); // 内容体
                httpContent.Headers.AddTraceInfoToHeader();        // 添加头部
                if (headerData != null)
                {
                    foreach (var header in headerData)
                    {
                        if (httpContent.Headers.Contains(header.Key)) continue;
                        httpContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                var cancellationTokenSource = new CancellationTokenSource();
                if (requestTimeout > 0) cancellationTokenSource.CancelAfter(requestTimeout);

                var httpRspMessage = await httpClient.SendAsync(new HttpRequestMessage
                {
                    Content    = httpContent,
                    Method     = HttpMethod.Get,
                    RequestUri = new Uri(string.IsNullOrWhiteSpace(postData) ? url : $"{url}?{postData}"),
                }, cancellationTokenSource.Token);

                var bytes  = await httpRspMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var result = encoding.GetString(bytes);
                trackEnd.SetHttpResponseBody(result);
                return result;
            }
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
        public static Task<string> PostAsync(string url, string postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null) => PostAsync(url, postData, null, encoding, contentType, requestTimeout, cookie);

        /// <summary>
        /// http request请求
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="headerData">头部</param>
        /// <param name="postData">查询条件</param>
        /// <param name="contentType">获取或设置 Content-type HTTP 标头的值。</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static async Task<string> PostAsync(string url, string postData, Dictionary<string, string> headerData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            using (var trackEnd = FsLinkTrack.TrackHttp(url, "POST", headerData, postData))
            {
                encoding ??= Encoding.UTF8;

                var httpContent = new StringContent(postData, encoding, contentType); // 内容体
                httpContent.Headers.AddTraceInfoToHeader();                           // 添加头部
                if (headerData != null)
                {
                    foreach (var header in headerData)
                    {
                        if (httpContent.Headers.Contains(header.Key)) continue;
                        httpContent.Headers.Add(header.Key, header.Value);
                    }
                }

                var cancellationTokenSource = new CancellationTokenSource();
                if (requestTimeout > 0) cancellationTokenSource.CancelAfter(requestTimeout);
                var httpRspMessage = httpClient.PostAsync(url, httpContent, cancellationTokenSource.Token);

                var bytes  = await (await httpRspMessage.ConfigureAwait(false)).Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var result = encoding.GetString(bytes);
                trackEnd.SetHttpResponseBody(result);
                return result;
            }
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
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null) => PostAsync(url, postData.Select(keyVal => $"{keyVal.Key}={keyVal.Value}").ToString("&"), null, encoding, contentType, requestTimeout, cookie);

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headerData">头部</param>
        /// <param name="postData">字典类型</param>
        /// <param name="contentType">获取或设置 Content-type HTTP 标头的值。</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            string pData;
            switch (contentType)
            {
                case "application/json":
                    pData = JsonConvert.SerializeObject(postData);
                    break;
                default:
                    pData = postData.Select(keyVal => $"{keyVal.Key}={keyVal.Value}").ToString("&");
                    break;
            }

            return PostAsync(url, pData, headerData, encoding, contentType, requestTimeout, cookie);
        }

        /// <summary>
        /// http request请求
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="headerData">头部</param>
        /// <param name="postData">查询条件</param>
        /// <param name="contentType">获取或设置 Content-type HTTP 标头的值。</param>
        /// <param name="requestTimeout">超时时间</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="cookie">是否需要cookie</param>
        public static async Task<string> PutAsync(string url, string postData, Dictionary<string, string> headerData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            using (var trackEnd = FsLinkTrack.TrackHttp(url, "POST", headerData, postData))
            {
                encoding ??= Encoding.UTF8;

                var httpContent = new StringContent(postData, encoding, contentType); // 内容体
                httpContent.Headers.AddTraceInfoToHeader();                           // 添加头部
                if (headerData != null)
                {
                    foreach (var header in headerData)
                    {
                        if (httpContent.Headers.Contains(header.Key)) continue;
                        httpContent.Headers.Add(header.Key, header.Value);
                    }
                }

                //httpContent.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                var cancellationTokenSource = new CancellationTokenSource();
                if (requestTimeout > 0) cancellationTokenSource.CancelAfter(requestTimeout);
                var httpRspMessage = httpClient.PutAsync(url, httpContent, cancellationTokenSource.Token);

                var bytes  = await (await httpRspMessage.ConfigureAwait(false)).Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var result = encoding.GetString(bytes);
                trackEnd.SetHttpResponseBody(result);
                return result;
            }
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
        public static Task<string> PutAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null) => PutAsync(url, postData.Select(keyVal => $"{keyVal.Key}={keyVal.Value}").ToString("&"), null, encoding, contentType, requestTimeout, cookie);
    }
}