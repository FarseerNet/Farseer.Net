 // ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     格式化变量
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     结果为True时，输出参数
        /// </summary>
        /// <param name="b">判断源结果</param>
        /// <param name="t">输出值</param>
        public static T IsTrue<T>(this bool b, T t)
        {
            return b ? t : default(T);
        }

        /// <summary>
        ///     结果为True时，输出参数
        /// </summary>
        /// <param name="b">判断源结果</param>
        /// <param name="t">输出值</param>
        public static T IsTrue<T>(this bool? b, T t)
        {
            return b.GetValueOrDefault() ? t : default(T);
        }

        /// <summary>
        ///     结果为False时，输出参数
        /// </summary>
        /// <param name="b">判断源结果</param>
        /// <param name="t">输出值</param>
        public static T IsFalse<T>(this bool b, T t)
        {
            return !b ? t : default(T);
        }

        /// <summary>
        ///     结果为False时，输出参数
        /// </summary>
        /// <param name="b">判断源结果</param>
        /// <param name="t">输出值</param>
        public static T IsFalse<T>(this bool? b, T t)
        {
            return !b.GetValueOrDefault() ? t : default(T);
        }

        /// <summary>
        ///     获取中文
        /// </summary>
        /// <param name="b"></param>
        /// <param name="strTrue">为True时的中文：是</param>
        /// <param name="strFalse">为False时的中文：否</param>
        /// <returns></returns>
        public static string GetName(this bool b, string strTrue = "是", string strFalse = "否")
        {
            return b ? strTrue : strFalse;
        }

        /// <summary>
        ///     获取中文
        /// </summary>
        /// <param name="b"></param>
        /// <param name="strTrue">为True时的中文：是</param>
        /// <param name="strFalse">为False时的中文：否</param>
        /// <returns></returns>
        public static string GetName(this bool? b, string strTrue = "是", string strFalse = "否")
        {
            return b.GetValueOrDefault() ? strTrue : strFalse;
        }
    }
}