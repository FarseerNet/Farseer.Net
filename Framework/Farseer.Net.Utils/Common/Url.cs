using System;
using System.Linq;
using System.Text.RegularExpressions;
using FS.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     解释Url
    /// </summary>
    public static class Url
    {
        /// <summary>
        ///     检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl"> 要验证的Url </param>
        /// <returns> 判断结果 </returns>
        public static bool IsUrl(string strUrl) => Regex.IsMatch(input: strUrl, pattern: @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");

        /// <summary>
        ///     返回完全域名，带端口
        /// </summary>
        /// <param name="url"> 来源URL </param>
        /// <returns> </returns>
        public static string GetDomain(string url)
        {
            url = url.Replace(oldValue: "\\", newValue: "/");
            
            url = Regex.Replace(input: url, pattern: "http://", replacement: "", options: RegexOptions.IgnoreCase);

            return "http://" + url.Substring(startIndex: 0, length: url.IndexOf(value: '/'));
        }

        /// <summary>
        ///     获取当前应用程序访问的域名(带端口)
        ///     www.xxx.com:81
        /// </summary>
        /// <param name="url"> 来源URL </param>
        /// <param name="node"> 要获取第几个节点，0：不限制 </param>
        public static string GetDomain(string url, int node)
        {
            var newUrl = url.Replace(oldValue: "\\", newValue: "/");
            newUrl = Regex.Replace(input: newUrl, pattern: "http://", replacement: "", options: RegexOptions.IgnoreCase);

            newUrl = newUrl.Substring(startIndex: 0, length: newUrl.IndexOf(value: '/'));

            if (node < 1) return url;

            var lstDomain = newUrl.ToList(defValue: "", splitString: ".");
            while (lstDomain.Count > node) lstDomain.RemoveAt(index: 0);
            return lstDomain.ToString(sign: ".");
        }

        /// <summary>
        ///     获得当前页面的名称
        /// </summary>
        public static string GetPageName(string url)
        {
            url = url.Split('?')[0];
            url = url.Substring(startIndex: url.LastIndexOf(value: '/') + 1);
            return url.Contains(value: '.') ? url : string.Empty;
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static string GetParams(string url)
        {
            if (string.IsNullOrWhiteSpace(value: url)) return string.Empty;
            var paramIndex = url.IndexOf(value: '?');
            if (paramIndex == -1 || url == "?") return url;
            return url.Substring(startIndex: paramIndex + 1);

            ////url = Webs.UrlDecode(url);
            ////清除重复的参数
            //List<string> lstParms = new List<string>();
            //foreach (var param in url.Substring(paramIndex + 1).Split('&'))
            //{
            //    string[] parmsSplit = param.Split('=');
            //    if (parmsSplit.Length == 1) { continue; }

            //    if (!lstParms.Exists(o => o.IsStartsWith(parmsSplit[0])))
            //    {
            //        lstParms.Add(string.Format("{0}={1}", parmsSplit[0], parmsSplit[1]));
            //    }
            //}
            //return lstParms.ToString("&");
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static string GetParm(string url, string paramName) => GetParm(url: url, paramName: paramName, defVal: string.Empty);

        /// <summary>
        ///     获取参数
        /// </summary>
        public static T GetParm<T>(string url, string paramName, T defVal)
        {
            var strParams = GetParams(url: url);
            foreach (var param in strParams.Split('&'))
            {
                var parmsSplit = param.Split('=');
                if (parmsSplit.Length == 1) continue;

                if (parmsSplit[0].IsEquals(str2: paramName)) return ConvertHelper.ConvertType(sourceValue: parmsSplit[1], defValue: defVal);
            }

            return defVal;
        }

        /// <summary>
        ///     将相对路径，转换成决对路径
        /// </summary>
        /// <param name="requestUrl"> http请求的地址 </param>
        /// <param name="url"> html获得的地址 </param>
        /// <returns> </returns>
        public static string ConvertUrlToDomain(string requestUrl, string url)
        {
            url = ConvertPath(path: url);
            // 绝对路径，直接返回
            if (!string.IsNullOrWhiteSpace(value: url) && url.ToLower().StartsWith(value: "http://")) return url;
            // 相对绝对路径，添加域名后返回
            if (!string.IsNullOrWhiteSpace(value: url) && url.ToLower().StartsWith(value: "/")) return GetDomain(url: requestUrl) + url;

            #region 相对路径 如../../

            // 深度
            var parentCount = 0;
            // 计算深度
            while (!string.IsNullOrWhiteSpace(value: url) && url.ToLower().StartsWith(value: "../"))
            {
                parentCount++;
                url = url.Substring(startIndex: 3);
            }

            // 初始化请求的地址
            if (requestUrl.EndsWith(value: "/") || requestUrl.LastIndexOf(value: '.') > requestUrl.LastIndexOf(value: '/')) requestUrl = requestUrl.DelLastOf(strChar: "/");
            //根据深度，得到目录
            while (parentCount > 0)
            {
                parentCount--;
                requestUrl = requestUrl.DelLastOf(strChar: "/");
            }

            return requestUrl + "/" + url;

            #endregion
        }

        /// <summary>
        ///     将\\转换成：/
        /// </summary>
        /// <returns> </returns>
        public static string ConvertPath(string path) => string.IsNullOrWhiteSpace(value: path) ? string.Empty : path.Replace(oldValue: "\\", newValue: "/");

        /// <summary>
        ///     通过正则，获取IP
        /// </summary>
        /// <param name="str"> </param>
        /// <returns> </returns>
        public static string GetIP(string str) => new Regex(pattern: "\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}").Match(input: str).Value;
    }
}