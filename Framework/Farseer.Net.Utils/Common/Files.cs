using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FS.Configuration;

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
        /// <param name="sbInputStream"> 文件流 </param>
        private static bool IsUtf8(FileStream sbInputStream)
        {
            int i;
            var bAllAscii = true;
            var iLen      = sbInputStream.Length;

            byte cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                var chr = (byte)sbInputStream.ReadByte();

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
                    if ((chr & 0xC0) != 0x80) return false;
                    cOctets--;
                }
            }

            return cOctets <= 0 && !bAllAscii;
        }

        /// <summary>
        ///     复制源文件夹下的所有内容到新文件夹
        /// </summary>
        /// <param name="sources"> 源文件夹路径 </param>
        /// <param name="dest"> 新文件夹路径 </param>
        public static void CopyFolder(string sources, string dest)
        {
            var dinfo = new DirectoryInfo(path: sources);
            //注，这里面传的是路径，并不是文件，所以不能包含带后缀的文件                
            foreach (var f in dinfo.GetFileSystemInfos())
            {
                //目标路径destName = 新文件夹路径 + 源文件夹下的子文件(或文件夹)名字                
                //Path.Combine(string a ,string b) 为合并两个字符串                     
                var destName = Path.Combine(path1: dest, path2: f.Name);
                if (f is FileInfo)
                {
                    //如果是文件就复制       
                    File.Copy(sourceFileName: f.FullName, destFileName: destName, overwrite: true); //true代表可以覆盖同名文件                     
                }
                else
                {
                    //如果是文件夹就创建文件夹，然后递归复制              
                    Directory.CreateDirectory(path: destName);
                    CopyFolder(sources: f.FullName, dest: destName);
                }
            }
        }

        /// <summary>
        ///     生成文件
        /// </summary>
        /// <param name="savePagePath"> 要保存的文件路径 </param>
        /// <param name="writeCode"> 生成文件所使用的编码 </param>
        /// <param name="strContent"> 要生成的内容 </param>
        public static bool WriteFile(string savePagePath, string writeCode, string strContent)
        {
            try
            {
                var enWriteCode  = Encoding.GetEncoding(name: writeCode); //生成文件所使用的编码方式
                var streamWriter = new StreamWriter(path: savePagePath, append: false, encoding: enWriteCode);
                streamWriter.Write(value: strContent);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     将/转换成：\\
        /// </summary>
        /// <returns> </returns>
        public static string ConvertPath(string path) => string.IsNullOrWhiteSpace(value: path) ? string.Empty : path.Replace(oldValue: "/", newValue: "\\");

        /// <summary>
        ///     获取根目录路径
        /// </summary>
        /// <returns> </returns>
        public static string GetRootPath() => ConvertPath(path: AppDomain.CurrentDomain.BaseDirectory) + "\\";

        //if (HttpContext.Current != null) { return HttpContext.Current.Request.PhysicalApplicationPath; }
        //else { return AppDomain.CurrentDomain.BaseDirectory; }
        /// <summary>
        ///     获取App_Data路径
        /// </summary>
        /// <returns> </returns>
        public static void CreateAppData()
        {
            if (!Directory.Exists(path: SysPath.AppData)) Directory.CreateDirectory(path: SysPath.AppData);
        }

        /// <summary>
        ///     获取文件夹容量
        /// </summary>
        /// <param name="dirPath"> 文件夹路径 </param>
        /// <returns> </returns>
        public static long GetDirLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(path: dirPath)) return 0;

            //定义一个DirectoryInfo对象
            var di = new DirectoryInfo(path: dirPath);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            var len = di.GetFiles().Sum(selector: fi => fi.Length);

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            var dis                 = di.GetDirectories();
            if (dis.Length > 0) len += dis.Sum(selector: t => GetDirLength(dirPath: t.FullName));
            return len;
        }

        /// <summary>
        ///     传入网页相对路径返回网页的html代码,出错返回null
        /// </summary>
        /// <param name="loadPagePath"> 源文件网页路径(不用带根目录路径) </param>
        /// <param name="encoding"> 读取源文件所使用的编码 </param>
        public static string GetFile(string loadPagePath, Encoding encoding)
        {
            var pageContent = ""; //源文件网页内容

            if (!File.Exists(path: loadPagePath)) return null;
            var streamReader = new StreamReader(path: loadPagePath, encoding: encoding);
            pageContent = streamReader.ReadToEnd(); // 读取文件
            streamReader.Close();

            return pageContent;
        }

        /// <summary>
        ///     根据文件路径得到文件的MD5值
        /// </summary>
        /// <param name="filePath"> 文件的路径 </param>
        /// <returns> MD5值 </returns>
        public static string GetMD5(string filePath)
        {
            try
            {
                var get_file  = new FileStream(path: filePath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read);
                var md5       = new MD5CryptoServiceProvider();
                var hash_byte = md5.ComputeHash(inputStream: get_file);
                var resule    = BitConverter.ToString(value: hash_byte);
                resule = resule.Replace(oldValue: "-", newValue: "");
                md5.Clear();
                md5.Dispose();
                get_file.Close();
                get_file.Dispose();
                return resule;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        ///     根据文件路径得到文件的MD5值
        /// </summary>
        /// <returns> MD5值 </returns>
        public static string GetMD5(FileStream fs)
        {
            try
            {
                var md5       = new MD5CryptoServiceProvider();
                var hash_byte = md5.ComputeHash(inputStream: fs);
                var resule    = BitConverter.ToString(value: hash_byte);
                resule = resule.Replace(oldValue: "-", newValue: "");
                return resule;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        ///     对文件重命名（不改变路径）
        /// </summary>
        /// <param name="dir"> 源文件 </param>
        /// <param name="newFileName"> 名称 </param>
        /// <param name="isChangeExtension"> 是否改变扩展名，为True时，根据newFileName的值进行改变。没有，则变更为无扩展的文件。为False时，则忽略newFileName的扩展名 </param>
        public static bool Rename(string dir, string newFileName, bool isChangeExtension = false)
        {
            //  源文件名
            var fileName = Path.GetFileNameWithoutExtension(path: newFileName);
            //  扩展名
            var extendName = isChangeExtension ? Path.GetExtension(path: newFileName) : Path.GetExtension(path: dir);
            //  组成新的文件名
            var newFile = Path.GetDirectoryName(path: dir) + "\\" + newFileName + extendName;

            // 新的文件名存在，则执行失败。
            if (File.Exists(path: newFile)) return false;

            // 移动
            File.Move(sourceFileName: dir, destFileName: newFile);

            return true;
        }
    }
}