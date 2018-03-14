using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FS.Utils.Common
{
    /// <summary>
    ///     ѹ���ַ���
    /// </summary>
    public class Compress
    {
        /// <summary>
        ///     ѹ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        public static string EnString(string str)
        {
            //��������ַ�������Base64����ת��ΪBase64,��ΪHTTP���������Base64��ᷢ��http 400����
            return Convert.ToBase64String(EnBytes(Convert.FromBase64String(Convert.ToBase64String(Encoding.Default.GetBytes(str)))));
        }

        /// <summary>
        ///     ��ѹ
        /// </summary>
        /// <param name="str">�ַ���</param>
        public static string DnString(string str)
        {
            return Encoding.Default.GetString(DeBytes(Convert.FromBase64String(str)));
        }

        /// <summary>
        ///     ѹ��
        /// </summary>
        /// <param name="bytes">�ֽ���</param>
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
        ///     ��ѹ
        /// </summary>
        /// <param name="bytes">�ֽ���</param>
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
        ///     ��ѹ�ļ�
        /// </summary>
        /// <param name="rarPath">���ѹ���ļ���װ����·��</param>
        /// <param name="filePath">ѹ�����ļ�·��</param>
        /// <param name="toPath">��ѹ��·��</param>
        /// <returns></returns>
        public static bool DeRar(string filePath, string toPath, string rarPath)
        {
            //ȡ��ϵͳ��ʱĿ¼
            //string sysTempDir = Path.GetTempPath();

            //Ҫ��ѹ���ļ�·��������������
            //string rarFilePath = @"d:\test.rar";

            //ȷ��Ҫ��ѹ����Ŀ¼����ϵͳ��ʱ�ļ����£���ԭѹ���ļ�ͬ����Ŀ¼��
            // string unrarDestPath = Path.Combine(sysTempDir,
            //     Path.GetFileNameWithoutExtension(rarFilePath));

            //��ϳ���Ҫshell��������ʽ
            //string shellArguments = string.Format("x -o+ \"{0}\" \"{1}\\\"",
            //    rarFilePath, unrarDestPath);

            Directory.CreateDirectory(toPath);

            var shellArguments = $"x -o+ \"{filePath}\" \"{toPath}\\\"";

            //��Process����
            using (var unrar = new Process())
            {
                unrar.StartInfo.FileName = rarPath;
                unrar.StartInfo.Arguments = shellArguments;
                //����rar����Ĵ���
                unrar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                unrar.Start();
                //�ȴ���ѹ���
                unrar.WaitForExit();
                unrar.Close();
            }
            return true;


            //ͳ�ƽ�ѹ���Ŀ¼���ļ���
            //DirectoryInfo di = new DirectoryInfo(unrarDestPath);

            //MessageBox.Show(string.Format("��ѹ��ɣ�����ѹ����{0}��Ŀ¼��{1}���ļ�",
            //    di.GetDirectories().Length, di.GetFiles().Length));
        }
    }
}