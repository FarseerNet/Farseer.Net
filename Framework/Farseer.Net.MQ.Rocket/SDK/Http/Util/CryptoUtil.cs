using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http.Util
{
    public interface ICryptoUtil
    {
        string HMACSign(string       data, string key, SigningAlgorithm algorithmName);
        byte[] HMACSignBinary(byte[] data, byte[] key, SigningAlgorithm algorithmName);

        byte[] ComputeMD5Hash(byte[] data);
        byte[] ComputeMD5Hash(Stream steam);
    }

    public static class CryptoUtilFactory
    {
        private static readonly CryptoUtil util = new CryptoUtil();

        public static ICryptoUtil CryptoInstance => util;

        private class CryptoUtil : ICryptoUtil
        {
            public string HMACSign(string data, string key, SigningAlgorithm algorithmName)
            {
                var binaryData = Encoding.UTF8.GetBytes(s: data);
                return HMACSign(data: binaryData, key: key, algorithmName: algorithmName);
            }

            public byte[] ComputeMD5Hash(byte[] data)
            {
                var hashed = MD5.Create().ComputeHash(buffer: data);
                return hashed;
            }

            public byte[] ComputeMD5Hash(Stream steam)
            {
                var hashed = MD5.Create().ComputeHash(inputStream: steam);
                return hashed;
            }

            public byte[] HMACSignBinary(byte[] data, byte[] key, SigningAlgorithm algorithmName)
            {
                if (key == null || key.Length == 0) throw new ArgumentNullException(paramName: "key", message: "Please specify a Secret Signing Key.");

                if (data == null || data.Length == 0) throw new ArgumentNullException(paramName: "data", message: "Please specify data to sign.");

                var algorithm = KeyedHashAlgorithm.Create(algName: algorithmName.ToString().ToUpper(culture: CultureInfo.InvariantCulture));
                if (null == algorithm) throw new InvalidOperationException(message: "Please specify a KeyedHashAlgorithm to use.");

                try
                {
                    algorithm.Key = key;
                    var bytes = algorithm.ComputeHash(buffer: data);
                    return bytes;
                }
                finally
                {
                    algorithm.Clear();
                }
            }

            public string HMACSign(byte[] data, string key, SigningAlgorithm algorithmName)
            {
                if (string.IsNullOrEmpty(value: key)) throw new ArgumentNullException(paramName: "key", message: "Please specify a Secret Signing Key.");

                if (data == null || data.Length == 0) throw new ArgumentNullException(paramName: "data", message: "Please specify data to sign.");

                var algorithm = KeyedHashAlgorithm.Create(algName: algorithmName.ToString().ToUpper(culture: CultureInfo.InvariantCulture));
                if (null == algorithm) throw new InvalidOperationException(message: "Please specify a KeyedHashAlgorithm to use.");

                try
                {
                    algorithm.Key = Encoding.UTF8.GetBytes(s: key);
                    var bytes = algorithm.ComputeHash(buffer: data);
                    return Convert.ToBase64String(inArray: bytes);
                }
                finally
                {
                    algorithm.Clear();
                }
            }
        }
    }
}