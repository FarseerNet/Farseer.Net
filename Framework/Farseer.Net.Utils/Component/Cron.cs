using System;
using System.Collections.Generic;
using System.Linq;
using FS.Extends;

namespace FS.Utils.Component
{
    /// <summary> 轻量级Cron表达式 </summary>
    /// <remarks>
    ///     基本构成：秒+分+时+天+月+星期
    ///     每段构成：
    ///     *全部，该类型片段全部可选
    ///     ?跳过
    ///     数字，具体某个数值可选
    ///     -区间，横杠表示的一个区间可选
    ///     逗号多选，逗号分隔的多个数字或区间可选
    ///     /步进，在上述可选数字内，间隔多少选一个
    /// </remarks>
    /// <example>
    ///     */2 每两秒一次
    ///     0,1,2 * * * * 每分钟的0秒1秒2秒各一次
    ///     5/20 * * * * 每分钟的5秒25秒45秒各一次
    ///     * 1-10,13,25/3 * * * 每小时的1分4分7分10分13分25分，每一秒各一次
    ///     0 0 0 1 * * 每个月1日的0点整
    ///     0 0 2 * * 1-5 每个工作日的凌晨2点
    /// </example>
    public class Cron
    {
        /// <summary> 秒数集合 </summary>
        public int[] Seconds;
        /// <summary> 分钟集合 </summary>
        public int[] Minutes;
        /// <summary> 小时集合 </summary>
        public int[] Hours;
        /// <summary> 日期集合 </summary>
        public int[] DaysOfMonth;
        /// <summary> 月份集合 </summary>
        public int[] Months;
        /// <summary> 星期集合 </summary>
        public int[] DaysOfWeek;
        private string _expression;

        /// <summary> 实例化Cron表达式 </summary>
        public Cron()
        {
        }

        /// <summary> 实例化Cron表达式 </summary>
        /// <param name="expression"> </param>
        public Cron(string expression)
        {
            _expression = expression;
        }

        /// <summary> 已重载。 </summary>
        /// <returns> </returns>
        public override string ToString() => _expression;

        /// <summary> 指定时间是否位于表达式之内 </summary>
        /// <param name="time"> </param>
        /// <returns> </returns>
        public bool IsTime(DateTime time) => Seconds.Contains(value: time.Second)  &&
                                             Minutes.Contains(value: time.Minute)  &&
                                             Hours.Contains(value: time.Hour)      &&
                                             DaysOfMonth.Contains(value: time.Day) &&
                                             Months.Contains(value: time.Month)    &&
                                             DaysOfWeek.Contains(value: (int)time.DayOfWeek);

        /// <summary> 分析表达式 </summary>
        /// <returns> </returns>
        public bool Parse()
        {
            var ss = _expression.Split(' ');
            if (ss.Length == 0) return false;

            if (!TryParse(value: ss[0], start: 0, max: 60, vs: out var vs)) return false;
            Seconds = vs;
            if (!TryParse(value: ss.Length > 1 ? ss[1] : "*", start: 0, max: 60, vs: out vs)) return false;
            Minutes = vs;
            if (!TryParse(value: ss.Length > 2 ? ss[2] : "*", start: 0, max: 24, vs: out vs)) return false;
            Hours = vs;
            if (!TryParse(value: ss.Length > 3 ? ss[3] : "*", start: 1, max: 32, vs: out vs)) return false;
            DaysOfMonth = vs;
            if (!TryParse(value: ss.Length > 4 ? ss[4] : "*", start: 1, max: 13, vs: out vs)) return false;
            Months = vs;
            if (!TryParse(value: ss.Length > 5 ? ss[5] : "*", start: 0, max: 7, vs: out vs)) return false;
            DaysOfWeek = vs;

            return true;
        }

        /// <summary> 分析表达式 </summary>
        /// <param name="expression"> </param>
        /// <returns> </returns>
        public bool Parse(string expression)
        {
            _expression = expression;
            return Parse();
        }

        private bool TryParse(string value, int start, int max, out int[] vs)
        {
            // 固定值，最为常见，优先计算
            if (int.TryParse(s: value, result: out var n))
            {
                vs = new[] { n };
                return true;
            }

            var rs = new List<int>();
            vs = null;

            // 递归处理混合值
            if (value.Contains(value: ','))
            {
                foreach (var item in value.Split(','))
                {
                    if (!TryParse(value: item, start: start, max: max, vs: out var arr)) return false;
                    if (arr.Length > 0) rs.AddRange(collection: arr);
                }

                vs = rs.ToArray();
                return true;
            }

            // 步进值
            var step = 1;
            var p    = value.IndexOf(value: '/');
            if (p > 0)
            {
                step  = value.Substring(startIndex: p + 1).ConvertType(defValue: 0);
                value = value.Substring(startIndex: 0, length: p);
            }

            // 连续范围
            var s = start;
            if (value == "*" || value == "?")
                s = 0;
            else if ((p = value.IndexOf(value: '-')) > 0)
            {
                s   = value.Substring(startIndex: 0, length: p).ConvertType(defValue: 0);
                max = value.Substring(startIndex: p + 1).ConvertType(defValue: 0) + 1;
            }
            else if (int.TryParse(s: value, result: out n))
                s = n;
            else
                return false;

            for (var i = s; i < max; i += step)
                if (i >= start)
                    rs.Add(item: i);

            vs = rs.ToArray();
            return true;
        }

        /// <summary> 获得指定时间之后的下一次执行时间，不含指定时间 </summary>
        /// <param name="time"> </param>
        /// <returns> </returns>
        public DateTime GetNext(DateTime time)
        {
            // 设置末尾，避免死循环越界
            var end = time.AddYears(value: 1);
            
            for (var dt = time.AddSeconds(value: 1); dt < end; dt = dt.AddSeconds(value: 1))
                if (IsTime(time: dt))
                    return dt;

            return DateTime.MinValue;
        }
    }
}