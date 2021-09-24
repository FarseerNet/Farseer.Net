using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;

namespace FS.Utils.Component
{
    /// <summary>
    ///     语言工具
    /// </summary>
    public abstract class Chinese
    {
        private static Hashtable _phrase;

        /// <summary>
        ///     包含字符 ASC 码的整形数组。
        /// </summary>
        private static readonly int[] pv = { -20319, -20317, -20304, -20295, -20292, -20283, -20265, -20257, -20242, -20230, -20051, -20036, -20032, -20026, -20002, -19990, -19986, -19982, -19976, -19805, -19784, -19775, -19774, -19763, -19756, -19751, -19746, -19741, -19739, -19728, -19725, -19715, -19540, -19531, -19525, -19515, -19500, -19484, -19479, -19467, -19289, -19288, -19281, -19275, -19270, -19263, -19261, -19249, -19243, -19242, -19238, -19235, -19227, -19224, -19218, -19212, -19038, -19023, -19018, -19006, -19003, -18996, -18977, -18961, -18952, -18783, -18774, -18773, -18763, -18756, -18741, -18735, -18731, -18722, -18710, -18697, -18696, -18526, -18518, -18501, -18490, -18478, -18463, -18448, -18447, -18446, -18239, -18237, -18231, -18220, -18211, -18201, -18184, -18183, -18181, -18012, -17997, -17988, -17970, -17964, -17961, -17950, -17947, -17931, -17928, -17922, -17759, -17752, -17733, -17730, -17721, -17703, -17701, -17697, -17692, -17683, -17676, -17496, -17487, -17482, -17468, -17454, -17433, -17427, -17417, -17202, -17185, -16983, -16970, -16942, -16915, -16733, -16708, -16706, -16689, -16664, -16657, -16647, -16474, -16470, -16465, -16459, -16452, -16448, -16433, -16429, -16427, -16423, -16419, -16412, -16407, -16403, -16401, -16393, -16220, -16216, -16212, -16205, -16202, -16187, -16180, -16171, -16169, -16158, -16155, -15959, -15958, -15944, -15933, -15920, -15915, -15903, -15889, -15878, -15707, -15701, -15681, -15667, -15661, -15659, -15652, -15640, -15631, -15625, -15454, -15448, -15436, -15435, -15419, -15416, -15408, -15394, -15385, -15377, -15375, -15369, -15363, -15362, -15183, -15180, -15165, -15158, -15153, -15150, -15149, -15144, -15143, -15141, -15140, -15139, -15128, -15121, -15119, -15117, -15110, -15109, -14941, -14937, -14933, -14930, -14929, -14928, -14926, -14922, -14921, -14914, -14908, -14902, -14894, -14889, -14882, -14873, -14871, -14857, -14678, -14674, -14670, -14668, -14663, -14654, -14645, -14630, -14594, -14429, -14407, -14399, -14384, -14379, -14368, -14355, -14353, -14345, -14170, -14159, -14151, -14149, -14145, -14140, -14137, -14135, -14125, -14123, -14122, -14112, -14109, -14099, -14097, -14094, -14092, -14090, -14087, -14083, -13917, -13914, -13910, -13907, -13906, -13905, -13896, -13894, -13878, -13870, -13859, -13847, -13831, -13658, -13611, -13601, -13406, -13404, -13400, -13398, -13395, -13391, -13387, -13383, -13367, -13359, -13356, -13343, -13340, -13329, -13326, -13318, -13147, -13138, -13120, -13107, -13096, -13095, -13091, -13076, -13068, -13063, -13060, -12888, -12875, -12871, -12860, -12858, -12852, -12849, -12838, -12831, -12829, -12812, -12802, -12607, -12597, -12594, -12585, -12556, -12359, -12346, -12320, -12300, -12120, -12099, -12089, -12074, -12067, -12058, -12039, -11867, -11861, -11847, -11831, -11798, -11781, -11604, -11589, -11536, -11358, -11340, -11339, -11324, -11303, -11097, -11077, -11067, -11055, -11052, -11045, -11041, -11038, -11024, -11020, -11019, -11018, -11014, -10838, -10832, -10815, -10800, -10790, -10780, -10764, -10587, -10544, -10533, -10519, -10331, -10329, -10328, -10322, -10315, -10309, -10307, -10296, -10281, -10274, -10270, -10262, -10260, -10256, -10254 };

        /// <summary>
        ///     包含汉字拼音的字符串数组。
        /// </summary>
        private static readonly string[] ps = { "a", "ai", "an", "ang", "ao", "ba", "bai", "ban", "bang", "bao", "bei", "ben", "beng", "bi", "bian", "biao", "bie", "bin", "bing", "bo", "bu", "ca", "cai", "can", "cang", "cao", "ce", "ceng", "cha", "chai", "chan", "chang", "chao", "che", "chen", "cheng", "chi", "chong", "chou", "chu", "chuai", "chuan", "chuang", "chui", "chun", "chuo", "ci", "cong", "cou", "cu", "cuan", "cui", "cun", "cuo", "da", "dai", "dan", "dang", "dao", "de", "deng", "di", "dian", "diao", "die", "ding", "diu", "dong", "dou", "du", "duan", "dui", "dun", "duo", "e", "en", "er", "fa", "fan", "fang", "fei", "fen", "feng", "fo", "fou", "fu", "ga", "gai", "gan", "gang", "gao", "ge", "gei", "gen", "geng", "gong", "gou", "gu", "gua", "guai", "guan", "guang", "gui", "gun", "guo", "ha", "hai", "han", "hang", "hao", "he", "hei", "hen", "heng", "hong", "hou", "hu", "hua", "huai", "huan", "huang", "hui", "hun", "huo", "ji", "jia", "jian", "jiang", "jiao", "jie", "jin", "jing", "jiong", "jiu", "ju", "juan", "jue", "jun", "ka", "kai", "kan", "kang", "kao", "ke", "ken", "keng", "kong", "kou", "ku", "kua", "kuai", "kuan", "kuang", "kui", "kun", "kuo", "la", "lai", "lan", "lang", "lao", "le", "lei", "leng", "li", "lia", "lian", "liang", "liao", "lie", "lin", "ling", "liu", "long", "lou", "lu", "lv", "luan", "lue", "lun", "luo", "ma", "mai", "man", "mang", "mao", "me", "mei", "men", "meng", "mi", "mian", "miao", "mie", "min", "ming", "miu", "mo", "mou", "mu", "na", "nai", "nan", "nang", "nao", "ne", "nei", "nen", "neng", "ni", "nian", "niang", "niao", "nie", "nin", "ning", "niu", "nong", "nu", "nv", "nuan", "nue", "nuo", "o", "ou", "pa", "pai", "pan", "pang", "pao", "pei", "pen", "peng", "pi", "pian", "piao", "pie", "pin", "ping", "po", "pu", "qi", "qia", "qian", "qiang", "qiao", "qie", "qin", "qing", "qiong", "qiu", "qu", "quan", "que", "qun", "ran", "rang", "rao", "re", "ren", "reng", "ri", "rong", "rou", "ru", "ruan", "rui", "run", "ruo", "sa", "sai", "san", "sang", "sao", "se", "sen", "seng", "sha", "shai", "shan", "shang", "shao", "she", "shen", "sheng", "shi", "shou", "shu", "shua", "shuai", "shuan", "shuang", "shui", "shun", "shuo", "si", "song", "sou", "su", "suan", "sui", "sun", "suo", "ta", "tai", "tan", "tang", "tao", "te", "teng", "ti", "tian", "tiao", "tie", "ting", "tong", "tou", "tu", "tuan", "tui", "tun", "tuo", "wa", "wai", "wan", "wang", "wei", "wen", "weng", "wo", "wu", "xi", "xia", "xian", "xiang", "xiao", "xie", "xin", "xing", "xiong", "xiu", "xu", "xuan", "xue", "xun", "ya", "yan", "yang", "yao", "ye", "yi", "yin", "ying", "yo", "yong", "you", "yu", "yuan", "yue", "yun", "za", "zai", "zan", "zang", "zao", "ze", "zei", "zen", "zeng", "zha", "zhai", "zhan", "zhang", "zhao", "zhe", "zhen", "zheng", "zhi", "zhong", "zhou", "zhu", "zhua", "zhuai", "zhuan", "zhuang", "zhui", "zhun", "zhuo", "zi", "zong", "zou", "zu", "zuan", "zui", "zun", "zuo" };

        /// <summary>
        ///     包含要排除处理的字符的字符串数组。
        /// </summary>
        private static string[] bd = { "，", "。", "“", "”", "‘", "’", "￥", "＄", "（", "「", "『", "）", "」", "』", "［", "〖", "【", "］", "〗", "】", "—", "…", "《", "＜", "》", "＞" };

        /// <summary>
        ///     设置或获取包含列外词组读音的键/值对的组合。
        /// </summary>
        public static Hashtable Phrase
        {
            get => _phrase ??
                   (_phrase = new Hashtable { { "重庆", "Chong Qing" }, { "深圳", "Shen Zhen" }, { "什么", "Shen Me" } });
            set => _phrase = value;
        }

        /// <summary>
        ///     将指定中文字符串转换为拼音形式。
        /// </summary>
        /// <param name="chs"> 要转换的中文字符串。 </param>
        /// <param name="separator"> 连接拼音之间的分隔符。 </param>
        /// <param name="initialCap"> 指定是否将首字母大写。 </param>
        /// <returns> 包含中文字符串的拼音的字符串。 </returns>
        public static string Convert(string chs, string separator = "", bool initialCap = false)
        {
            if (string.IsNullOrEmpty(value: chs)) return "";
            if (string.IsNullOrEmpty(value: separator)) separator = "";
            // 例外词组
            chs = Phrase.Cast<DictionaryEntry>().Aggregate(seed: chs, func: (current, de) => current.Replace(oldValue: de.Key.ToString(), newValue: string.Format(format: " {0} ", arg0: de.Value.ToString().Replace(oldValue: " ", newValue: separator))));
            var returnstr = "";
            var b         = false;
            var nowchar   = chs.ToCharArray();
            var ci        = Thread.CurrentThread.CurrentCulture;
            var ti        = ci.TextInfo;
            for (var j = 0; j < nowchar.Length; j++)
            {
                var array = Encoding.Default.GetBytes(s: nowchar[j].ToString());
                var s     = nowchar[j].ToString();

                if (array.Length == 1)
                {
                    b         =  true;
                    returnstr += s;
                }
                else
                {
                    if (s == "？")
                    {
                        if (returnstr == "" || b)
                            returnstr += s;
                        else
                            returnstr += separator + s;
                        continue;
                    }

                    int i1     = array[0];
                    int i2     = array[1];
                    var chrasc = i1 * 256 + i2 - 65536;
                    for (var i = pv.Length - 1; i >= 0; i--)
                    {
                        if (pv[i] > chrasc) continue;
                        s = ps[i];
                        if (initialCap) s = ti.ToTitleCase(str: s);
                        if (returnstr == "" || b)
                            returnstr += s;
                        else
                            returnstr += separator + s;
                        break;
                    }

                    b = false;
                }
            }

            returnstr = returnstr.Replace(oldValue: " ", newValue: separator);
            return returnstr;
        }

        /// <summary>
        ///     转半角的函数(DBC case)
        /// </summary>
        /// <param name="input"> </param>
        /// <returns> </returns>
        public static string ToDbc(string input)
        {
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }

                if (c[i] > 65280 && c[i] < 65375) c[i] = (char)(c[i] - 65248);
            }

            return new string(value: c);
        }

#if !CORE
        /// <summary>
        ///     转换为简体中文
        /// </summary>
        public static string ToSChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }

        /// <summary>
        ///     转换为繁体中文
        /// </summary>
        public static string ToTChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        }
#endif
        /// <summary>
        ///     将全角数字转换为数字
        /// </summary>
        /// <param name="sbcCase"> </param>
        /// <returns> </returns>
        public static string SbcCaseToNumberic(string sbcCase)
        {
            var c = sbcCase.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                var b = Encoding.Unicode.GetBytes(chars: c, index: i, count: 1);
                if (b.Length != 2) continue;
                if (b[1]     != 255) continue;
                b[0] = (byte)(b[0] + 32);
                b[1] = 0;
                c[i] = Encoding.Unicode.GetChars(bytes: b)[0];
            }

            return new string(value: c);
        }

        /// <summary>
        ///     转全角的函数(SBC case)
        /// </summary>
        /// <param name="input"> </param>
        /// <returns> </returns>
        public static string ToSbc(string input)
        {
            //半角转全角： 
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }

                if (c[i] < 127) c[i] = (char)(c[i] + 65248);
            }

            return new string(value: c);
        }

        /// <summary>
        ///     获取随机中文
        /// </summary>
        /// <param name="strlength"> </param>
        /// <returns> </returns>
        public static string GetRandomChinese(int strlength)
        {
            // 获取GB2312编码页（表） 
            var gb = Encoding.GetEncoding(name: "gb2312");

            var bytes = CreateRegionCode(strlength: strlength);

            var sb = new StringBuilder();

            for (var i = 0; i < strlength; i++)
            {
                var temp = gb.GetString(bytes: (byte[])System.Convert.ChangeType(value: bytes[i], conversionType: typeof(byte[])));
                sb.Append(value: temp);
            }

            return sb.ToString();
        }

        /* 
        此函数在汉字编码范围内随机创建含两个元素的十六进制字节数组，每个字节数组代表一个汉字，并将 
        四个字节数组存储在object数组中。 
        参数：strlength，代表需要产生的汉字个数 
        */

        private static object[] CreateRegionCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            var rBase = new string[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

            var rnd = new Random();

            //定义一个object数组用来 
            var bytes = new object[strlength];

            /*
             每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bytes数组中 
             每个汉字有四个区位码组成 
             区位码第1位和区位码第2位作为字节数组第一个元素 
             区位码第3位和区位码第4位作为字节数组第二个元素 
            */
            for (var i = 0; i < strlength; i++)
            {
                //区位码第1位 
                var r1     = rnd.Next(minValue: 11, maxValue: 14);
                var str_r1 = rBase[r1].Trim();

                //区位码第2位 
                rnd = new Random(Seed: r1 * unchecked((int)DateTime.Now.Ticks) + i); // 更换随机数发生器的 种子避免产生重复值 
                int r2;
                if (r1 == 13)
                    r2 = rnd.Next(minValue: 0, maxValue: 7);
                else
                    r2 = rnd.Next(minValue: 0, maxValue: 16);
                var str_r2 = rBase[r2].Trim();

                //区位码第3位 
                rnd = new Random(Seed: r2 * unchecked((int)DateTime.Now.Ticks) + i);
                var r3     = rnd.Next(minValue: 10, maxValue: 16);
                var str_r3 = rBase[r3].Trim();

                //区位码第4位 
                rnd = new Random(Seed: r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                    r4 = rnd.Next(minValue: 1, maxValue: 16);
                else if (r3 == 15)
                    r4 = rnd.Next(minValue: 0, maxValue: 15);
                else
                    r4 = rnd.Next(minValue: 0, maxValue: 16);
                var str_r4 = rBase[r4].Trim();

                // 定义两个字节变量存储产生的随机汉字区位码 
                var byte1 = System.Convert.ToByte(value: str_r1 + str_r2, fromBase: 16);
                var byte2 = System.Convert.ToByte(value: str_r3 + str_r4, fromBase: 16);
                // 将两个字节变量存储在字节数组中 
                var str_r = new[] { byte1, byte2 };

                // 将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(value: str_r, index: i);
            }

            return bytes;
        }
    }
}