using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FS.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     下载文件
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        ///     下载文件到服务器
        /// </summary>
        /// <param name="url"> 请求地址 </param>
        /// <param name="savePath"> 保存路径 </param>
        /// <param name="wc"> </param>
        public static int Save(string url, string savePath, WebClient wc = null)
        {
            if (string.IsNullOrWhiteSpace(value: url)) return 0;
            url = url.Replace(oldValue: "\\", newValue: "/");

            int fileSize;
            var isNew = wc == null;
            if (wc == null)
            {
                wc = new WebClient { Proxy = null };
                wc.Headers.Add(name: "Accept", value: "*/*");
                wc.Headers.Add(name: "Referer", value: url);
                wc.Headers.Add(name: "Cookie", value: "bid=\"YObnALe98pw\";");
                wc.Headers.Add(name: "User-Agent", value: "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
            }

            try
            {
                wc.DownloadFile(address: url, fileName: savePath);
                var f = new FileInfo(fileName: savePath);
                fileSize = (int)f.Length;
            }
            finally
            {
                if (!isNew)
                    SetCookies(wc: wc);
                else
                    wc.Dispose();
            }

            return fileSize;
        }

        /// <summary>
        ///     判断网络文件是否存在
        /// </summary>
        /// <param name="url"> 要读取的网页URL </param>
        /// <param name="encoding"> 读取源文件所使用的编码 </param>
        public static bool IsHaving(string url, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(value: url)) return false;
            url = url.Replace(oldValue: "\\", newValue: "/");
            if (encoding == null) encoding = Encoding.UTF8;

            bool isHaving;
            try
            {
                using (var web = new WebClient())
                {
                    web.Proxy = null;
                    web.Headers.Add(name: "Accept", value: "*/*");
                    web.Headers.Add(name: "Referer", value: url);
                    web.Headers.Add(name: "Cookie", value: "bid=\"YObnALe98pw\"");
                    web.Headers.Add(name: "User-Agent", value: "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.5 Safari/537.31");
                    isHaving = web.DownloadData(address: url).Length > 0;
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
            var setcookie = wc.ResponseHeaders[header: HttpResponseHeader.SetCookie];
            if (string.IsNullOrEmpty(value: setcookie)) return;
            var cookie     = wc.Headers[header: HttpRequestHeader.Cookie];
            var cookieList = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(value: cookie))
            {
                foreach (var ck in cookie.Split(';'))
                {
                    var key   = ck.Substring(startIndex: 0, length: ck.IndexOf(value: '='));
                    var value = ck.Substring(startIndex: ck.IndexOf(value: '=') + 1);
                    if (!cookieList.ContainsKey(key: key)) cookieList.Add(key: key, value: value);
                }
            }

            foreach (var ck in setcookie.Split(';'))
            {
                var str                                                                                       = ck;
                while (str.Contains(value: ',') && str.IndexOf(value: ',') < str.LastIndexOf(value: '=')) str = str.Substring(startIndex: str.IndexOf(value: ',') + 1);
                var key                                                                                       = str.IndexOf(value: '=') != -1 ? str.Substring(startIndex: 0, length: str.IndexOf(value: '=')) : "";
                var value                                                                                     = str.Substring(startIndex: str.IndexOf(value: '=') + 1);
                if (!cookieList.ContainsKey(key: key))
                    cookieList.Add(key: key, value: value);
                else
                    cookieList[key: key] = value;
            }

            var list  = new string[cookieList.Count()];
            var index = 0;
            foreach (var pair in cookieList)
            {
                list[index] = $"{pair.Key}={pair.Value}";
                index++;
            }

            wc.Headers[header: HttpRequestHeader.Cookie] = list.ToString(sign: ";");
        }
    }
}