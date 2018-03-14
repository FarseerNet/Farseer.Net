using System;

namespace FS.Utils.Common
{
    /// <summary>
    ///     随机数工具
    /// </summary>
    public abstract class Rand
    {
        private static readonly Random rand = new Random(DateTime.Now.Second);

        /// <summary>
        ///     返回非负随机数
        /// </summary>
        /// <returns></returns>
        public static int GetRandom()
        {
            return rand.Next();
        }

        /// <summary>
        ///     返回一个小于所指定最大值的非负随机数。
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int GetRandom(int maxValue)
        {
            return rand.Next(maxValue + 1);
        }

        /// <summary>
        ///     返回一个指定范围内的随机数。
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int GetRandom(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue + 1);
        }

        /// <summary>
        ///     随机生成字符串
        /// </summary>
        /// <returns></returns>
        public static string CreateRandomString(int length)
        {
            var checkCode = String.Empty;
            for (var i = 0; i < length; i++)
            {
                //随机产生一个整数                             
                var number = rand.Next();
                //如果随机数是偶数 取余选择从[0-9]                             
                char code;
                if (number % 2 == 0)
                    code = (char)('0' + (char)(number % 10));
                else
                    //如果随机数是奇数 选择从[A-Z]                                    
                    code = (char)('A' + (char)(number % 26));
                checkCode += code.ToString();
            }
            return checkCode;
        }
    }
}