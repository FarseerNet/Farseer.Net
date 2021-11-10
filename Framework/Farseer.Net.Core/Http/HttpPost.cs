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
    public class HttpPost
    {
        private static readonly HttpClient httpClient = new(handler: new HttpClientHandler
        {
            UseProxy                                  = true,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });

        static HttpPost()
        {
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            //httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
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
        public static Task<string> PostAsync(string url, string postData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null) => PostAsync(url: url, postData: postData, headerData: null, encoding: encoding, contentType: contentType, requestTimeout: requestTimeout, cookie: cookie);

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<TResult> PostAsync<TResult>(string url, string postData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            var result = await PostAsync(url: url, postData: postData, headerData: null, contentType: contentType, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);
            return Jsons.ToObject<TResult>(result);
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
        public static async Task<string> PostAsync(string url, string postData, Dictionary<string, string> headerData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
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
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="contentType"> 获取或设置 Content-type HTTP 标头的值。 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<TResult> PostAsync<TResult>(string url, string postData, Dictionary<string, string> headerData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            var result = await PostAsync(url, postData, headerData, contentType, requestTimeout, encoding, cookie);
            return Jsons.ToObject<TResult>(result);
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
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
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

            return PostAsync(url: url, postData: pData, headerData: null, contentType: contentType, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);
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
        public static async Task<TResult> PostAsync<TResult>(string url, Dictionary<string, string> postData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            var result = await PostAsync(url, postData, contentType, requestTimeout, encoding, cookie);
            return Jsons.ToObject<TResult>(result);
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
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
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

            return PostAsync(url: url, postData: pData, headerData: headerData, contentType: contentType, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);
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
        public static async Task<TResult> PostAsync<TResult>(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, string contentType = "application/x-www-form-urlencoded", int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            var result = await PostAsync(url, postData, headerData, contentType, requestTimeout,encoding, cookie);
            return Jsons.ToObject<TResult>(result);
        }
    }
}