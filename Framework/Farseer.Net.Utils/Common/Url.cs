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
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        ///     返回完全域名，带端口
        /// </summary>
        /// <param name="url">来源URL</param>
        /// <returns></returns>
        public static string GetDomain(string url)
        {
            var newUrl = url.Replace("\\", "/");
            newUrl = Regex.Replace(newUrl, "http://", "", RegexOptions.IgnoreCase);

            return "http://" + newUrl.Substring(0, newUrl.IndexOf('/'));
        }

        /// <summary>
        ///     获取当前应用程序访问的域名(带端口)
        ///     www.xxx.com:81
        /// </summary>
        /// <param name="url">来源URL</param>
        /// <param name="node">要获取第几个节点，0：不限制</param>
        public static string GetDomain(string url, int node)
        {
            var newUrl = url.Replace("\\", "/");
            newUrl = Regex.Replace(newUrl, "http://", "", RegexOptions.IgnoreCase);

            newUrl = newUrl.Substring(0, newUrl.IndexOf('/'));

            if (node < 1) { return url; }

            var lstDomain = newUrl.ToList("", ".");
            while (lstDomain.Count > node) { lstDomain.RemoveAt(0); }
            return lstDomain.ToString(".");
        }

        /// <summary>
        ///     获得当前页面的名称
        /// </summary>
        public static string GetPageName(string url)
        {
            url = url.Split('?')[0];
            url = url.Substring(url.LastIndexOf('/') + 1);
            return url.Contains<char>('.') ? url : string.Empty;
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static string GetParams(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) { return string.Empty; }
            var paramIndex = url.IndexOf('?');
            if (paramIndex == -1 || url == "?") { return url; }
            return url.Substring(paramIndex + 1);

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
        public static string GetParm(string url, string paramName)
        {
            return GetParm(url, paramName, string.Empty);
        }

        /// <summary>
        ///     获取参数
        /// </summary>
        public static T GetParm<T>(string url, string paramName, T defVal)
        {
            var strParams = GetParams(url);
            foreach (var param in strParams.Split('&'))
            {
                var parmsSplit = param.Split('=');
                if (parmsSplit.Length == 1) { continue; }

                if (parmsSplit[0].IsEquals(paramName)) { return ConvertHelper.ConvertType(parmsSplit[1], defVal); }
            }

            return defVal;
        }

        /// <summary>
        ///     将相对路径，转换成决对路径
        /// </summary>
        /// <param name="requestUrl">http请求的地址</param>
        /// <param name="url">html获得的地址</param>
        /// <returns></returns>
        public static string ConvertUrlToDomain(string requestUrl, string url)
        {
            url = ConvertPath(url);
            // 绝对路径，直接返回
            if (!string.IsNullOrWhiteSpace(url) && url.ToLower().StartsWith("http://")) { return url; }
            // 相对绝对路径，添加域名后返回
            if (!string.IsNullOrWhiteSpace(url) && url.ToLower().StartsWith("/")) { return GetDomain(requestUrl) + url; }

            #region 相对路径 如../../

            // 深度
            var parentCount = 0;
            // 计算深度
            while (!string.IsNullOrWhiteSpace(url) && url.ToLower().StartsWith("../"))
            {
                parentCount++;
                url = url.Substring(3);
            }
            // 初始化请求的地址
            if (requestUrl.EndsWith("/") || requestUrl.LastIndexOf('.') > requestUrl.LastIndexOf('/')) { requestUrl = requestUrl.DelLastOf("/"); }
            //根据深度，得到目录
            while (parentCount > 0)
            {
                parentCount--;
                requestUrl = requestUrl.DelLastOf("/");
            }
            return requestUrl + "/" + url;

            #endregion
        }

        /// <summary>
        ///     将\\转换成：/
        /// </summary>
        /// <returns></returns>
        public static string ConvertPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? string.Empty : path.Replace("\\", "/");
        }

        /// <summary>
        ///     通过正则，获取IP
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetIP(string str)
        {
            return new Regex("\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}").Match(str).Value;
        }
    }
}