// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-05-30 20:53
// ********************************************

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FS.Extends;

namespace FS.Core
{
    /// <summary>
    ///     数据分页列表及总数
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    [DataContract]
    public class DataSplitList<TEntity> where TEntity : class
    {
        public DataSplitList()
        {
            
        }
        
        /// <summary>
        ///     数据分页列表及总数
        /// </summary>
        public DataSplitList(List<TEntity> list, long totalCount)
        {
            List       = list;
            TotalCount = totalCount;
        }
        /// <summary>
        ///     数据分页列表及总数
        /// </summary>
        public DataSplitList(IEnumerable<TEntity> list, long totalCount)
        {
            List       = list.ToList();
            TotalCount = totalCount;
        }

        /// <summary>
        ///     总页数
        /// </summary>
        [DataMember]
        public long TotalCount { get; set; }

        /// <summary>
        ///     数据列表
        /// </summary>
        [DataMember]
        public List<TEntity> List { get; set; }
    }
}