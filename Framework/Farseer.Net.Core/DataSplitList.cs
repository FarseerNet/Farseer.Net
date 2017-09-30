// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-05-30 20:53
// ********************************************

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Farseer.Net.Core
{
    /// <summary>
    /// 数据分页列表及总数
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [DataContract]
    public class DataSplitList<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据分页列表及总数
        /// </summary>
        public DataSplitList(List<TEntity> list, int totalCount)
        {
            List = list;
            TotalCount = totalCount;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        [DataMember]
        public List<TEntity> List { get; set; }
    }
}