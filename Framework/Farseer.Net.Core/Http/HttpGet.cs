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

namespace FS.Core.Http
{
    public class HttpGet
    {
        private static readonly HttpClient httpClient = new(handler: new HttpClientHandler
        {
            UseProxy                                  = true,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });

        static HttpGet()
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
        public static Task<string> GetAsync(string url, Dictionary<string, string> postData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null) => GetAsync(url: url, postData: postData.Select(selector: keyVal => $"{keyVal.Key}={keyVal.Value}").ToString(sign: "&"), headerData: null, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<TResult> GetAsync<TResult>(string url, Dictionary<string, string> postData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            var result = await GetAsync(url: url, postData: postData.Select(selector: keyVal => $"{keyVal.Key}={keyVal.Value}").ToString(sign: "&"), headerData: null, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);
            return Jsons.ToObject<TResult>(result);
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="headerData"> 添加头部信息 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<string> GetAsync(string url, string postData = null, Dictionary<string, string> headerData = null, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
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
                trackEnd.SetHttpResponseBody(responseBody: result, (int)httpRspMessage.StatusCode);
                return result;
            }
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="headerData"> 添加头部信息 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<TResult> GetAsync<TResult>(string url, string postData = null, Dictionary<string, string> headerData = null, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            var result = await GetAsync(url, postData, headerData, requestTimeout, encoding, cookie);
            return Jsons.ToObject<TResult>(result);
        }
    }
}