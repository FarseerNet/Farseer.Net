using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FS.Core.Http
{
    public class HttpPostJson
    {
        private const string ContentType = "application/json";

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, string postData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null) => HttpPost.PostAsync(url: url, postData: postData, headerData: null, ContentType, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static async Task<string> TryPostAsync(string url, string postData, string defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync(url: url, postData: postData, headerData: null, ContentType, requestTimeout: requestTimeout, encoding: encoding, cookie: cookie);
            }
            catch
            {
                return defaultResult;
            }
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<TResult> PostAsync<TResult>(string url, string postData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync<TResult>(url, postData, ContentType, requestTimeout, encoding, cookie);
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<TResult> TryPostAsync<TResult>(string url, string postData, TResult defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync<TResult>(url, postData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }


        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, string postData, Dictionary<string, string> headerData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
        }

        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<string> TryPostAsync(string url, string postData, Dictionary<string, string> headerData, string defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }


        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<TResult> PostAsync<TResult>(string url, string postData, Dictionary<string, string> headerData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync<TResult>(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
        }
        /// <summary>
        ///     http request请求
        /// </summary>
        /// <param name="url"> 资源地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 查询条件 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<TResult> TryPostAsync<TResult>(string url, string postData, Dictionary<string, string> headerData, TResult defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync<TResult>(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }


        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync(url, postData, ContentType, requestTimeout, encoding, cookie);
        }
        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<string> TryPostAsync(string url, Dictionary<string, string> postData, string defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync(url, postData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<TResult> PostAsync<TResult>(string url, Dictionary<string, string> postData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync<TResult>(url, postData, ContentType, requestTimeout, encoding, cookie);
        }
        
        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<TResult> TryPostAsync<TResult>(string url, Dictionary<string, string> postData, TResult defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync<TResult>(url, postData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }


        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<string> PostAsync(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<string> TryPostAsync(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, string defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        public static Task<TResult> PostAsync<TResult>(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            return HttpPost.PostAsync<TResult>(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
        }

        /// <summary>
        ///     以Post方式请求远程URL
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="headerData"> 头部 </param>
        /// <param name="postData"> 字典类型 </param>
        /// <param name="requestTimeout"> 超时时间 </param>
        /// <param name="encoding"> 编码格式 </param>
        /// <param name="cookie"> 是否需要cookie </param>
        /// <param name="defaultResult">异常时默认返回值 </param>
        public static async Task<TResult> TryPostAsync<TResult>(string url, Dictionary<string, string> postData, Dictionary<string, string> headerData, TResult defaultResult, int requestTimeout = 0, Encoding encoding = null, CookieContainer cookie = null)
        {
            try
            {
                return await HttpPost.PostAsync<TResult>(url, postData, headerData, ContentType, requestTimeout, encoding, cookie);
            }
            catch
            {
                return defaultResult;
            }
        }
    }
}