using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Farseer.Net.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     加密工具
    /// </summary>
    public abstract class Encrypt
    {
        /// <summary>
        ///     MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="isReverse">是否加密后反转字符串</param>
        /// <param name="encoding">默认编码Encoding.Default</param>
        /// <param name="toUpper">是否加密后转为大写</param>
        /// <param name="count">加密次数</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str, Encoding encoding = null, bool toUpper = false, bool isReverse = false, int count = 1)
        {
            if (encoding == null) { encoding = Encoding.Default; }

            // MD5.Create().ComputeHash(encoding.GetBytes(str))
            var bytes = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(str));
            var md5 = string.Empty;
            for (var i = 0; i < bytes.Length; i++) { md5 += bytes[i].ToString("x").PadLeft(2, '0'); }

            if (isReverse) { md5 = Str.Reverse(md5); }
            if (toUpper) { md5 = md5.ToUpper(); }

            if (count <= 1) { return md5; }
            return MD5(md5, encoding, toUpper, isReverse, --count);
        }
        /// <summary>
        ///     MD5后返回Base64String函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="isReverse">是否加密后反转字符串</param>
        /// <param name="encoding">默认编码Encoding.Default</param>
        /// <param name="toUpper">是否加密后转为大写</param>
        /// <param name="count">加密次数</param>
        /// <returns>MD5结果</returns>
        public static string MD5Base64String(string str, Encoding encoding = null, bool toUpper = false, bool isReverse = false, int count = 1)
        {
            if (encoding == null) { encoding = Encoding.UTF8; }

            var bytes = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(str));
            var val = Convert.ToBase64String(bytes);

            if (isReverse) { val = Str.Reverse(val); }
            if (toUpper) { val = val.ToUpper(); }

            if (count <= 1) { return val; }
            return MD5Base64String(val, encoding, toUpper, isReverse, --count);
        }

        /// <summary>
        ///     SHA256函数
        /// </summary>
        /// ///
        /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string Sha256(string str)
        {
            var sha256Data = Encoding.UTF8.GetBytes(str);
            var sha256 = new SHA256Managed();
            var result = sha256.ComputeHash(sha256Data);
            return Convert.ToBase64String(result); //返回长度为44字节的字符串
        }

        /// <summary>
        ///     加密
        /// </summary>
        public class AES
        {
            //默认密钥向量
            private static readonly byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

            /// <summary>
            ///     AES加密字符串
            /// </summary>
            /// <param name="encryptString">待加密的字符串</param>
            /// <param name="encryptKey">加密密钥,要求为8位</param>
            /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
            public static string Encode(string encryptString, string encryptKey)
            {
                encryptKey = encryptKey.SubString(0, 32);
                encryptKey = encryptKey.PadRight(32, ' ');

                var rijndaelProvider = new RijndaelManaged { Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32)), IV = Keys };
                var rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

                var inputData = Encoding.UTF8.GetBytes(encryptString);
                var encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Convert.ToBase64String(encryptedData);
            }

            /// <summary>
            ///     AES解密字符串
            /// </summary>
            /// <param name="decryptString">待解密的字符串</param>
            /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
            /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
            public static string Decode(string decryptString, string decryptKey)
            {
                try
                {
                    decryptKey = decryptKey.SubString(0, 32);
                    decryptKey = decryptKey.PadRight(32, ' ');

                    var rijndaelProvider = new RijndaelManaged { Key = Encoding.UTF8.GetBytes(decryptKey), IV = Keys };
                    var rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                    var inputData = Convert.FromBase64String(decryptString);
                    var decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                    return Encoding.UTF8.GetString(decryptedData);
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        ///     加密
        /// </summary>
        public class DES
        {
            //默认密钥向量
            private static readonly byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            /// <summary>
            ///     DES加密字符串
            /// </summary>
            /// <param name="encryptString">待加密的字符串</param>
            /// <param name="encryptKey">加密密钥,要求为8位</param>
            /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
            public static string Encode(string encryptString, string encryptKey)
            {
                encryptKey = encryptKey.SubString(0, 8);
                encryptKey = encryptKey.PadRight(8, ' ');
                var rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                var rgbIv = Keys;
                var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var dCsp = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCsp.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }

            /// <summary>
            ///     DES解密字符串
            /// </summary>
            /// <param name="decryptString">待解密的字符串</param>
            /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
            /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
            public static string Decode(string decryptString, string decryptKey)
            {
                try
                {
                    decryptKey = decryptKey.SubString(0, 8);
                    decryptKey = decryptKey.PadRight(8, ' ');
                    var rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                    var rgbIv = Keys;
                    var inputByteArray = Convert.FromBase64String(decryptString);
                    var dcsp = new DESCryptoServiceProvider();

                    var mStream = new MemoryStream();
                    var cStream = new CryptoStream(mStream, dcsp.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
                catch
                {
                    return "";
                }
            }
        }

    }
}