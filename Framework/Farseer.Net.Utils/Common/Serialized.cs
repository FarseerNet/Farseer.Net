using System.IO;
using System.Xml.Serialization;

namespace FS.Utils.Common
{
    /// <summary>
    ///     SerializationHelper 的摘要说明。
    /// </summary>
    public abstract class Serialized<T>
    {
        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="filePath"> 文件路径 </param>
        /// <returns> </returns>
        public static T Load(string filePath)
        {
            if (!File.Exists(path: filePath)) return default;

            FileStream fs = null;
            try
            {
                fs = new FileStream(path: filePath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.ReadWrite);
                var serializer = new XmlSerializer(type: typeof(T));
                return (T)serializer.Deserialize(stream: fs);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }


        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="t"> 对象 </param>
        /// <param name="filePath"> 文件路径 </param>
        public static bool Save(T t, string filePath)
        {
            var        succeed = false;
            FileStream fs      = null;
            try
            {
                Directory.CreateDirectory(path: filePath);
                fs = new FileStream(path: filePath, mode: FileMode.Create, access: FileAccess.Write, share: FileShare.ReadWrite);
                var serializer = new XmlSerializer(type: t.GetType());
                serializer.Serialize(stream: fs, o: t);
                succeed = true;
            }
            finally
            {
                if (fs != null) fs.Close();
            }

            return succeed;
        }
    }
}