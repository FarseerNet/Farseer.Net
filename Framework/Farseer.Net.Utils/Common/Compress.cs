using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FS.Utils.Common
{
    /// <summary>
    ///     压缩字符串
    /// </summary>
    public class Compress
    {
        /// <summary>
        ///     压缩
        /// </summary>
        /// <param name="str">字符串</param>
        public static string EnString(string str)
        {
            //因输入的字符串不是Base64所以转换为Base64,因为HTTP如果不传递Base64则会发生http 400错误
            return Convert.ToBase64String(EnBytes(Convert.FromBase64String(Convert.ToBase64String(Encoding.Default.GetBytes(str)))));
        }

        /// <summary>
        ///     解压
        /// </summary>
        /// <param name="str">字符串</param>
        public static string DnString(string str)
        {
            return Encoding.Default.GetString(DeBytes(Convert.FromBase64String(str)));
        }

        /// <summary>
        ///     压缩
        /// </summary>
        /// <param name="bytes">字节组</param>
        public static byte[] EnBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                var compress = new GZipStream(ms, CompressionMode.Compress);
                compress.Write(bytes, 0, bytes.Length);
                compress.Close();
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     解压
        /// </summary>
        /// <param name="bytes">字节组</param>
        public static byte[] DeBytes(byte[] bytes)
        {
            using (var tempMs = new MemoryStream())
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var decompress = new GZipStream(ms, CompressionMode.Decompress);
                    decompress.CopyTo(tempMs);
                    decompress.Close();
                    return tempMs.ToArray();
                }
            }
        }

        /// <summary>
        ///     解压文件
        /// </summary>
        /// <param name="rarPath">解决压缩文件安装程序路径</param>
        /// <param name="filePath">压缩包文件路径</param>
        /// <param name="toPath">解压到路径</param>
        /// <returns></returns>
        public static bool DeRar(string filePath, string toPath, string rarPath)
        {
            //取得系统临时目录
            //string sysTempDir = Path.GetTempPath();

            //要解压的文件路径，请自行设置
            //string rarFilePath = @"d:\test.rar";

            //确定要解压到的目录，是系统临时文件夹下，与原压缩文件同名的目录里
            // string unrarDestPath = Path.Combine(sysTempDir,
            //     Path.GetFileNameWithoutExtension(rarFilePath));

            //组合出需要shell的完整格式
            //string shellArguments = string.Format("x -o+ \"{0}\" \"{1}\\\"",
            //    rarFilePath, unrarDestPath);

            Directory.CreateDirectory(toPath);

            var shellArguments = $"x -o+ \"{filePath}\" \"{toPath}\\\"";

            //用Process调用
            using (var unrar = new Process())
            {
                unrar.StartInfo.FileName = rarPath;
                unrar.StartInfo.Arguments = shellArguments;
                //隐藏rar本身的窗口
                unrar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                unrar.Start();
                //等待解压完成
                unrar.WaitForExit();
                unrar.Close();
            }
            return true;


            //统计解压后的目录和文件数
            //DirectoryInfo di = new DirectoryInfo(unrarDestPath);

            //MessageBox.Show(string.Format("解压完成，共解压出：{0}个目录，{1}个文件",
            //    di.GetDirectories().Length, di.GetFiles().Length));
        }
    }
}