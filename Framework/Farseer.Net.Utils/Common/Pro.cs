using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace FS.Utils.Common
{
    /// <summary>
    ///     Process管理
    /// </summary>
    public class Pro
    {
        /// <summary>
        ///     根据默认浏览器打开网页
        /// </summary>
        /// <param name="url">要打开的链接</param>
        /// <param name="openInNewWindow">是否在新窗口打开</param>
        /// <returns></returns>
        public static bool OpenUrl(string url, bool openInNewWindow)
        {
            try
            {
                const string name = @"http\shell\open\command";
                var openSubKey = Registry.ClassesRoot.OpenSubKey(name, false);
                if (openSubKey != null)
                {
                    var fileName = ((string)openSubKey.GetValue(null, null)).Split(new[] { '"' })[1];
                    if (openInNewWindow)
                    {
                        var process = new Process
                        {
                            StartInfo =
                            {
                                FileName = fileName,
                                Arguments = url
                            }
                        };
                        process.Start();
                        return true;
                    }
                    Process.Start(fileName, url);
                }
            }
            catch (Exception)
            {
                try { Process.Start(url); return true; }
                catch (Exception) { return false; }
            }
            return true;
        }
    }
}