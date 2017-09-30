using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Farseer.Net.Context;
using Farseer.Net.Extends;

namespace Farseer.Net.Utils.Common
{
    /// <summary>
    ///     下载文件
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        ///     下载文件到服务器
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="wc"></param>
        public static int Save(string url, string savePath, WebClient wc = null)
        {
            if (string.IsNullOrWhiteSpace(url)) { return 0; }
            url = url.Replace("\\", "/");

            int fileSize;
            var isNew = wc == null;
            if (wc == null)
            {
                wc = new WebClient { Proxy = null };
                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Referer", url);
                wc.Headers.Add("Cookie", "bid=\"YObnALe98pw\";");
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
            }

            try
            {
                wc.DownloadFile(url, savePath);
                var f = new FileInfo(savePath);
                fileSize = (int)f.Length;
            }
            finally
            {
                if (!isNew) { SetCookies(wc); }
                else
                { wc.Dispose(); }
            }
            return fileSize;
        }

        /// <summary>
        ///     判断网络文件是否存在
        /// </summary>
        /// <param name="url">要读取的网页URL</param>
        /// <param name="encoding">读取源文件所使用的编码</param>
        public static bool IsHaving(string url, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(url)) { return false; }
            url = url.Replace("\\", "/");
            if (encoding == null) { encoding = Encoding.UTF8; }

            bool isHaving;
            try
            {
                using (var web = new WebClient())
                {
                    web.Proxy = null;
                    web.Headers.Add("Accept", "*/*");
                    web.Headers.Add("Referer", url);
                    web.Headers.Add("Cookie", "bid=\"YObnALe98pw\"");
                    web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
                    isHaving = web.DownloadData(url).Length > 0;
                }
            }
            catch
            {
                isHaving = false;
            }
            return isHaving;
        }

        /// <summary>
        ///     把服務器返回的Cookies信息寫入到客戶端中
        /// </summary>
        private static void SetCookies(WebClient wc)
        {
            if (wc.ResponseHeaders == null) return;
            var setcookie = wc.ResponseHeaders[HttpResponseHeader.SetCookie];
            if (String.IsNullOrEmpty(setcookie)) return;
            var cookie = wc.Headers[HttpRequestHeader.Cookie];
            var cookieList = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(cookie))
            {
                foreach (var ck in cookie.Split(';'))
                {
                    var key = ck.Substring(0, ck.IndexOf('='));
                    var value = ck.Substring(ck.IndexOf('=') + 1);
                    if (!cookieList.ContainsKey(key)) cookieList.Add(key, value);
                }
            }

            foreach (var ck in setcookie.Split(';'))
            {
                var str = ck;
                while (Enumerable.Contains(str, ',') && str.IndexOf(',') < str.LastIndexOf('=')) { str = str.Substring(str.IndexOf(',') + 1); }
                var key = str.IndexOf('=') != -1 ? str.Substring(0, str.IndexOf('=')) : "";
                var value = str.Substring(str.IndexOf('=') + 1);
                if (!cookieList.ContainsKey(key)) { cookieList.Add(key, value); }
                else
                { cookieList[key] = value; }
            }

            var list = new string[cookieList.Count()];
            var index = 0;
            foreach (var pair in cookieList)
            {
                list[index] = $"{pair.Key}={pair.Value}";
                index++;
            }

            wc.Headers[HttpRequestHeader.Cookie] = list.ToString(";");
        }
    }
}