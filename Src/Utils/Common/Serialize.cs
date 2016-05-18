using System;
using System.IO;
using System.Xml.Serialization;
using FS.Log;

namespace FS.Utils.Common
{
    /// <summary>
    ///     （反）序例化操作
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        ///     反序列化（从指定路径中读取内容并转换成T）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="isErrorToMove">错误时移动当前文件</param>
        public static T Load<T>(string filePath, string fileName, bool isErrorToMove = false) where T : class, new()
        {
            if (!File.Exists(filePath + fileName)) { return default(T); }

            try
            {
                using (var fs = new FileStream(filePath + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                if (fileName.ToLower() != "system.config") { LogManger.Log.Error(ex); } // 如果当前是system.config报错，则会导致当前死循环。}
                if (!isErrorToMove) { return null; }
                File.Move(filePath + fileName, filePath + fileName + ".bak_" + DateTime.Now.ToString("yyMMdd-HHmmss"));
                return default(T);
            }
        }

        /// <summary>
        ///     序列化（将T保存到指定路径中）
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        public static void Save<T>(T t, string filePath, string fileName) where T : class, new()
        {
            // 创建目录
            Directory.CreateDirectory(string.IsNullOrWhiteSpace(fileName) ? filePath.Replace("/", "\\").Substring(0, filePath.LastIndexOf("\\")) : filePath); //filePath.Substring(0, filePath.LastIndexOf("\\", StringComparison.Ordinal))

            using (var fs = new FileStream(filePath + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(t.GetType());
                serializer.Serialize(fs, t);
            }
        }
    }
}