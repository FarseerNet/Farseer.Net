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
        private static readonly HttpClient httpClient = new(handler: new HttpClientHandler
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
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> GetAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, int requestTimeout = 0, CookieContainer cookie = null) => GetAsync(url: url, postData: postData.Select(selector: keyVal => $"{keyVal.Key}={keyVal.Value}").ToString(sign: "&"), headerData: null, encoding: encoding, requestTimeout: requestTimeout, cookie: cookie);

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="headerData"> 添加头部信息 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<string> GetAsync(string url, string postData = null, Dictionary<string, string> headerData = null, Encoding encoding = null, int requestTimeout = 0, CookieContainer cookie = null)
        {
            using (var trackEnd = FsLinkTrack.TrackHttp(url: url, method: "GET", headerData: headerData, requestBody: postData))
            {
                encoding ??= Encoding.UTF8;

                var httpContent = new StringContent(content: "", encoding: encoding); // 内容体
                httpContent.Headers.AddTraceInfoToHeader();                           // 添加头部
                if (headerData != null)
                {
                    foreach (var header in headerData)
                    {
                        if (httpContent.Headers.Contains(name: header.Key)) continue;
                        httpContent.Headers.TryAddWithoutValidation(name: header.Key, value: header.Value);
                    }
                }

                var cancellationTokenSource = new CancellationTokenSource();
                if (requestTimeout > 0) cancellationTokenSource.CancelAfter(millisecondsDelay: requestTimeout);

                var httpRspMessage = await httpClient.SendAsync(request: new HttpRequestMessage
                {
                    Content    = httpContent,
                    Method     = HttpMethod.Get,
                    RequestUri = new Uri(uriString: string.IsNullOrWhiteSpace(value: postData) ? url : $"{url}?{postData}")
                }, cancellationToken: cancellationTokenSource.Token);

                var bytes  = await httpRspMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(continueOnCapturedContext: false);
                var result = encoding.GetString(bytes: bytes);
                trackEnd.SetHttpResponseBody(responseBody: result);
                return result;
            }
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, string postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null) => PostAsync(url: url, postData: postData, headerData: null, encoding: encoding, contentType: contentType, requestTimeout: requestTimeout, cookie: cookie);

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<string> PostAsync(string url, string postData, Dictionary<string, string> headerData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            using (var trackEnd = FsLinkTrack.TrackHttp(url: url, method: "POST", headerData: headerData, requestBody: postData))
            {
                encoding ??= Encoding.UTF8;

                var httpContent = new StringContent(content: postData, encoding: encoding, mediaType: contentType); // 内容体
                httpContent.Headers.AddTraceInfoToHeader();                                                         // 添加头部
                if (headerData != null)
                {
                    foreach (var header in headerData)
                    {
                        if (httpContent.Headers.Contains(name: header.Key)) continue;
                        httpContent.Headers.Add(name: header.Key, value: header.Value);
                    }
                }

                var cancellationTokenSource = new CancellationTokenSource();
                if (requestTimeout > 0) cancellationTokenSource.CancelAfter(millisecondsDelay: requestTimeout);
                var httpRspMessage = httpClient.PostAsync(requestUri: url, content: httpContent, cancellationToken: cancellationTokenSource.Token);

                var bytes  = await (await httpRspMessage.ConfigureAwait(continueOnCapturedContext: false)).Content.ReadAsByteArrayAsync().ConfigureAwait(continueOnCapturedContext: false);
                var result = encoding.GetString(bytes: bytes);
                trackEnd.SetHttpResponseBody(responseBody: result);
                return result;
            }
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            string pData;
            switch (contentType)
            {
                case "application/json":
                    pData = JsonConvert.SerializeObject(value: postData);
                    break;
                default:
                    pData = postData.Select(selector: keyVal => $"{keyVal.Key}={keyVal.Value}").ToString(sign: "&");
                    break;
            }

            return PostAsync(url: url, postData: pData, headerData: null, encoding: encoding, contentType: contentType, requestTimeout: requestTimeout, cookie: cookie);
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            string pData;
            switch (contentType)
            {
                case "application/json":
                    pData = JsonConvert.SerializeObject(value: postData);
                    break;
                default:
                    pData = postData.Select(selector: keyVal => $"{keyVal.Key}={keyVal.Value}").ToString(sign: "&");
                    break;
            }

            return PostAsync(url: url, postData: pData, headerData: headerData, encoding: encoding, contentType: contentType, requestTimeout: requestTimeout, cookie: cookie);
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<string> PutAsync(string url, string postData, Dictionary<string, string> headerData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null)
        {
            using (var trackEnd = FsLinkTrack.TrackHttp(url: url, method: "POST", headerData: headerData, requestBody: postData))
            {
                encoding ??= Encoding.UTF8;

                var httpContent = new StringContent(content: postData, encoding: encoding, mediaType: contentType); // 内容体
                httpContent.Headers.AddTraceInfoToHeader();                                                         // 添加头部
                if (headerData != null)
                {
                    foreach (var header in headerData)
                    {
                        if (httpContent.Headers.Contains(name: header.Key)) continue;
                        httpContent.Headers.Add(name: header.Key, value: header.Value);
                    }
                }

                //httpContent.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                var cancellationTokenSource = new CancellationTokenSource();
                if (requestTimeout > 0) cancellationTokenSource.CancelAfter(millisecondsDelay: requestTimeout);
                var httpRspMessage = httpClient.PutAsync(requestUri: url, content: httpContent, cancellationToken: cancellationTokenSource.Token);

                var bytes  = await (await httpRspMessage.ConfigureAwait(continueOnCapturedContext: false)).Content.ReadAsByteArrayAsync().ConfigureAwait(continueOnCapturedContext: false);
                var result = encoding.GetString(bytes: bytes);
                trackEnd.SetHttpResponseBody(responseBody: result);
                return result;
            }
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PutAsync(string url, Dictionary<string, string> postData, Encoding encoding = null, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, CookieContainer cookie = null) => PutAsync(url: url, postData: postData.Select(selector: keyVal => $"{keyVal.Key}={keyVal.Value}").ToString(sign: "&"), headerData: null, encoding: encoding, contentType: contentType, requestTimeout: requestTimeout, cookie: cookie);
    }
}