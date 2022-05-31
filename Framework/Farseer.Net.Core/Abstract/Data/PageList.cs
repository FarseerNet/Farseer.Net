﻿// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-05-30 20:53
// ********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Collections.Pooled;

namespace FS.Core.Abstract.Data
{
    /// <summary>
    ///     数据分页列表及总数
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    [DataContract]
    public class PageList<TEntity> : IPageList, IDisposable
    {
        public PageList()
        {
            List = new();
        }

        /// <summary>
        ///     数据分页列表及总数
        /// </summary>
        public PageList(PooledList<TEntity> list, long recordCount)
        {
            List        = list;
            RecordCount = recordCount;
        }

        /// <summary>
        ///     数据分页列表及总数
        /// </summary>
        public PageList(IEnumerable<TEntity> list, long recordCount)
        {
            List        = list.ToPooledList();
            RecordCount = recordCount;
        }

        /// <summary>
        ///     总页数
        /// </summary>
        [DataMember]
        public long RecordCount { get; set; }

        /// <summary>
        ///     数据列表
        /// </summary>
        [DataMember]
        public PooledList<TEntity> List { get; set; }

        public IEnumerable GetList() => List;
        
        public void Dispose()
        {
            List.Dispose();
        }
    }
}