using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FS.Configuration;
using FS.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     文件工具
    /// </summary>
    public static class Files
    {
        /// <summary>
        ///     判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        private static bool IsUtf8(FileStream sbInputStream)
        {
            int i;
            var bAllAscii = true;
            var iLen = sbInputStream.Length;

            byte cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                var chr = (byte) sbInputStream.ReadByte();

                if ((chr & 0x80) != 0) bAllAscii = false;

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80) { return false; }
                    cOctets--;
                }
            }

            return cOctets <= 0 && !bAllAscii;
        }

        /// <summary>
        ///     建立文件夹
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static void CreateDir(string path)
        {
            path = ConvertPath(path);
            if (string.IsNullOrWhiteSpace(path) || path.Trim() == "\\") { return; }

            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        }

        ///// <summary>
        /////     智能创建文件目录(任何级别目录)
        ///// </summary>
        ///// <param name="dirPath">路径</param>
        //public static bool CreateDirs(string dirPath)
        //{
        //    dirPath = ConvertPath(dirPath);

        //    if (Directory.Exists(dirPath))
        //    {
        //        return true;
        //    }

        //    var lstPath = dirPath.ToList(string.Empty, "\\");
        //    if (lstPath.GetLast().IndexOf('.') > -1 || lstPath.GetLast().IsNullOrEmpty())
        //    {
        //        lstPath.RemoveAt(lstPath.Count - 1);
        //    }

        //    var path = new StringBuilder();
        //    foreach (var str in lstPath)
        //    {
        //        path.Append(str + "\\");
        //        if (!Directory.Exists(path.ToString()))
        //        {
        //            CreateDir(path.ToString());
        //        }
        //    }
        //    return true;
        //}

        /// <summary>
        ///     删除目录,同时删除子目录所有文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static List<string> DeleteDir(string path)
        {
            var lst = new List<string>() {path};
            if (!Directory.Exists(path)) { return lst; }

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
                lst.Add(file);
            }
            foreach (var dir in dirs) { lst.AddRange(DeleteDir(dir)); }
            Directory.Delete(path, false);
            return lst;
        }

        /// <summary>
        ///     复制文件夹内的文件到指定路径
        /// </summary>
        /// <param name="srcPath">源文件夹</param>
        /// <param name="aimPath">目录文件夹</param>
        /// <param name="isCopySubDir">true:复制子文件夹;false:只复制根文件夹下的文件</param>
        /// <param name="overCopy">是复制覆盖</param>
        /// <param name="filterExtension">后缀名过滤，格式："svn|aspx|asp|exe"</param>
        public static void CopyDir(string srcPath, string aimPath, bool isCopySubDir = true, bool overCopy = true, string filterExtension = "")
        {
            aimPath = ConvertPath(aimPath);
            if (!aimPath.EndsWith("\\")) { aimPath += "\\"; }

            if (!Directory.Exists(aimPath)) { Directory.CreateDirectory(aimPath); }

            var lstFilter = filterExtension.ToList(string.Empty, "|");

            var fileList = Directory.GetFileSystemEntries(srcPath);

            // 遍历所有的文件和目录
            foreach (var file in fileList)
            {
                if (lstFilter.Exists(o => o == Path.GetExtension(file))) { continue; }

                if (Directory.Exists(file) && !isCopySubDir) { continue; }

                if (Directory.Exists(file) && isCopySubDir) { CopyDir(file, aimPath + Path.GetFileName(file), isCopySubDir, overCopy, filterExtension); }

                else
                { File.Copy(file, aimPath + Path.GetFileName(file), overCopy); }
            }
        }

        /// <summary>
        ///     生成文件
        /// </summary>
        /// <param name="savePagePath">要保存的文件路径</param>
        /// <param name="writeCode">生成文件所使用的编码</param>
        /// <param name="strContent">要生成的内容</param>
        public static bool WriteFile(string savePagePath, string writeCode, string strContent)
        {
            try
            {
                var enWriteCode = Encoding.GetEncoding(writeCode); //生成文件所使用的编码方式
                var streamWriter = new StreamWriter(savePagePath, false, enWriteCode);
                streamWriter.Write(strContent);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        ///     将/转换成：\\
        /// </summary>
        /// <returns></returns>
        public static string ConvertPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? string.Empty : path.Replace("/", "\\");
        }

        /// <summary>
        ///     获取根目录路径
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
        {
            return ConvertPath(AppDomain.CurrentDomain.BaseDirectory) + "\\";
            //if (HttpContext.Current != null) { return HttpContext.Current.Request.PhysicalApplicationPath; }
            //else { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        ///     获取App_Data路径
        /// </summary>
        /// <returns></returns>
        public static void CreateAppData()
        {
            if (!Directory.Exists(SysPath.AppData)) { Directory.CreateDirectory(SysPath.AppData); }
        }

        /// <summary>
        ///     获取文件夹容量
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns></returns>
        public static long GetDirLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath)) { return 0; }

            //定义一个DirectoryInfo对象
            var di = new DirectoryInfo(dirPath);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            var len = di.GetFiles().Sum(fi => fi.Length);

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            var dis = di.GetDirectories();
            if (dis.Length > 0) { len += dis.Sum(t => GetDirLength(t.FullName)); }
            return len;
        }

        /// <summary>
        ///     传入网页相对路径返回网页的html代码,出错返回null
        /// </summary>
        /// <param name="loadPagePath">源文件网页路径(不用带根目录路径)</param>
        /// <param name="encoding">读取源文件所使用的编码</param>
        public static string GetFile(string loadPagePath, Encoding encoding)
        {
            var pageContent = ""; //源文件网页内容

            if (!File.Exists(loadPagePath)) { return null; }
            var streamReader = new StreamReader(loadPagePath, encoding);
            pageContent = streamReader.ReadToEnd(); // 读取文件
            streamReader.Close();

            return pageContent;
        }

        /// <summary>
        ///     根据文件路径得到文件的MD5值
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5(string filePath)
        {
            try
            {
                var get_file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var md5 = new MD5CryptoServiceProvider();
                var hash_byte = md5.ComputeHash(get_file);
                var resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                md5.Clear();
                md5.Dispose();
                get_file.Close();
                get_file.Dispose();
                return resule;
            }
            catch (Exception e) {
                return e.ToString();
            }
        }

        /// <summary>
        ///     根据文件路径得到文件的MD5值
        /// </summary>
        /// <returns>MD5值</returns>
        public static string GetMD5(FileStream fs)
        {
            try
            {
                var md5 = new MD5CryptoServiceProvider();
                var hash_byte = md5.ComputeHash(fs);
                var resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                return resule;
            }
            catch (Exception e) {
                return e.ToString();
            }
        }

        /// <summary>
        ///     对文件重命名（不改变路径）
        /// </summary>
        /// <param name="dir">源文件</param>
        /// <param name="newFileName">名称</param>
        /// <param name="isChangeExtension">是否改变扩展名，为True时，根据newFileName的值进行改变。没有，则变更为无扩展的文件。为False时，则忽略newFileName的扩展名</param>
        public static bool Rename(string dir, string newFileName, bool isChangeExtension = false)
        {
            //  源文件名
            var fileName = Path.GetFileNameWithoutExtension(newFileName);
            //  扩展名
            var extendName = isChangeExtension ? Path.GetExtension(newFileName) : Path.GetExtension(dir);
            //  组成新的文件名
            var newFile = Path.GetDirectoryName(dir) + "\\" + newFileName + extendName;

            // 新的文件名存在，则执行失败。
            if (File.Exists(newFile)) { return false; }

            // 移动
            File.Move(dir, newFile);

            return true;
        }
    }
}