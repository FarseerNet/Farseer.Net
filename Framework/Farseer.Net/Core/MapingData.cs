// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-09-01 16:15
// ********************************************

using System.Collections.Generic;

namespace FS.Core
{
    /// <summary>
    /// 数据映射
    /// </summary>
    public class MapingData
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 该列的下的数据集合
        /// </summary>
        public List<object> DataList { get; set; }
    }
}