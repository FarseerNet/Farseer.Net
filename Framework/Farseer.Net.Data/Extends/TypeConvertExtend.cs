﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Collections.Pooled;
using FS.Cache;
using FS.Core.AOP.LinkTrack;
using FS.Core.Mapping.Attribute;
using FS.Data.Cache;
using FS.Data.Internal;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader"> 源IDataReader </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static PooledList<TEntity> ToList<TEntity>(this DbDataReader reader)
        {
            using var mapData = DataReaderHelper.DataReaderToDictionary(reader: reader);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (PooledList<TEntity>)StaticMethodCacheManger.Cache(type: type, methodName: "ToList", mapData);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader"> 源IDataReader </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static async Task<PooledList<TEntity>> ToListAsync<TEntity>(this Task<DbDataReader> reader)
        {
            using var mapData = DataReaderHelper.DataReaderToDictionary(reader: await reader);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (PooledList<TEntity>)StaticMethodCacheManger.Cache(type: type, methodName: "ToList", mapData);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader"> 源IDataReader </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static TEntity ToEntity<TEntity>(this IDataReader reader)
        {
            using var mapData = DataReaderHelper.DataReaderToDictionary(reader: reader);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (TEntity)StaticMethodCacheManger.Cache(type: type, methodName: "ToEntity", mapData, 0);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader"> 源IDataReader </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static TEntity ToEntity<TEntity>(this DataTable reader)
        {
            using var mapData = DataReaderHelper.DataTableToDictionary(dt: reader);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (TEntity)StaticMethodCacheManger.Cache(type: type, methodName: "ToEntity", mapData, 0);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader"> 源IDataReader </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static async Task<TEntity> ToEntityAsync<TEntity>(this Task<DataTable> reader)
        {
            using var mapData = DataReaderHelper.DataTableToDictionary(dt: await reader);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (TEntity)StaticMethodCacheManger.Cache(type: type, methodName: "ToEntity", mapData, 0);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     DataTable转换为List实体类
        /// </summary>
        /// <param name="dt"> 源DataTable </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static PooledList<TEntity> ToList<TEntity>(this DataTable dt)
        {
            using var mapData = DataReaderHelper.DataTableToDictionary(dt: dt);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (PooledList<TEntity>)StaticMethodCacheManger.Cache(type: type, methodName: "ToList", mapData);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     DataTable转换为List实体类
        /// </summary>
        /// <param name="dt"> 源DataTable </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static async Task<PooledList<TEntity>> ToListAsync<TEntity>(this Task<DataTable> dt)
        {
            using var mapData = DataReaderHelper.DataTableToDictionary(dt: await dt);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return (PooledList<TEntity>)StaticMethodCacheManger.Cache(type: type, methodName: "ToList", mapData);
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     DataTable转换为数组实体类
        /// </summary>
        /// <param name="dt"> 源DataTable </param>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static TEntity[] ToArray<TEntity>(this DataTable dt)
        {
            using var mapData = DataReaderHelper.DataTableToDictionary(dt: dt);
            var       type    = DynamicCompilationEntity.Instance.GetProxyType(entityType: typeof(TEntity));
            try
            {
                return ((PooledList<TEntity>)StaticMethodCacheManger.Cache(type: type, methodName: "ToList", mapData)).ToArray();
            }
            finally
            {
                foreach (var data in mapData)
                {
                    data.Dispose();
                }
            }
        }

        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="ds"> 源DataSet </param>
        /// <typeparam name="T"> 实体类 </typeparam>
        [TrackKeyLocation]
        public static PooledList<T> ToList<T>(this DataSet ds) where T : class, new() => ds.Tables.Count == 0 ? null : ds.Tables[index: 0].ToList<T>();

        /// <summary>
        ///     将XML转成实体
        /// </summary>
        [TrackKeyLocation]
        public static IEnumerable<TEntity> ToList<TEntity>(this XElement element) where TEntity : class
        {
            var orm = SetMapCacheManger.Cache(key: typeof(TEntity));

            foreach (var el in element.Elements())
            {
                var t = (TEntity)Activator.CreateInstance(type: typeof(TEntity));

                //赋值字段
                foreach (var kic in orm.MapList)
                {
                    var type = kic.Key.PropertyType;
                    if (!kic.Key.CanWrite) continue;

                    //switch (kic.Value.PropertyExtend)
                    {
                        //case eumPropertyExtend.Attribute:
                        //    if (el.Attribute(kic.Value.Name) == null) { continue; }
                        //kic.Key.SetValue(t, el.Attribute(kic.Value.Name).Value.ConvertType(type), null);
                        //    PropertySetCacheManger.Cache(kic.Key, t, el.Attribute(kic.Value.Name).Value.ConvertType(type));

                        //    break;
                        // case eumPropertyExtend.Element:
                        if (el.Element(name: kic.Value.Field.Name) == null) continue;

                        //kic.Key.SetValue(t, el.Element(kic.Value.Name).Value.ConvertType(type), null);
                        PropertySetCacheManger.Cache(key: kic.Key, instance: t, value: el.Element(name: kic.Value.Field.Name).Value.ConvertType(defType: type));
                        break;
                    }
                }

                yield return t;
            }
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="lst"> 集合 </param>
        /// <returns> </returns>
        public static DataTable ToTable<TEntity>(this IEnumerable<TEntity> lst) where TEntity : class
        {
            var dt = new DataTable();
            if (!lst.Any()) return dt;

            var map       = SetMapCacheManger.Cache(key: typeof(TEntity));
            var lstFields = map.MapList.Where(predicate: o => o.Value.Field.StorageType != EumStorageType.Ignore).OrderBy(keySelector: o => o.Value.Field.FieldIndex).ToList();
            // 添加DataTable列
            foreach (var field in lstFields)
            {
                // 对   List 类型处理
                var type              = field.Key.PropertyType.GetNullableArguments();
                if (type.IsEnum) type = typeof(int);

                var col = new DataColumn(columnName: field.Value.Field.Name, dataType: type) { AutoIncrement = field.Value.Field.IsDbGenerated };
                dt.Columns.Add(column: col);
            }

            foreach (var info in lst)
            {
                var dr = dt.NewRow();
                foreach (var field in lstFields)
                {
                    var value = EntityHelper.GetValue(entity: info, propertyName: field.Key.Name, defValue: (object)null);
                    var type  = field.Key.PropertyType.GetNullableArguments();
                    // 枚举特殊处理
                    if (type.IsEnum) value = (int)(value ?? 0);

                    dr[columnName: field.Value.Field.Name] = value == null && !type.IsClass || field.Value.Field.IsDbGenerated ? 0 : value;
                }

                dt.Rows.Add(row: dr);
            }

            return dt;
        }
    }
}