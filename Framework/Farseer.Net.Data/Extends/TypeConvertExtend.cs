using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml.Linq;
using FS.Cache;
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
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this DbDataReader reader)
        {
            var mapData = DataReaderHelper.DataReaderToDictionary(reader);
            var type = new EntityDynamics().BuildType(typeof(TEntity));
            return (List<TEntity>)InstanceStaticCacheManger.Cache(type, "ToList", (object)mapData);
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToEntity<TEntity>(this IDataReader reader)
        {
            var mapData = DataReaderHelper.DataReaderToDictionary(reader);
            var type = new EntityDynamics().BuildType(typeof(TEntity));
            return (TEntity)InstanceStaticCacheManger.Cache(type, "ToEntity", (object)mapData, 0);
        }

        /// <summary>
        ///     DataTable转换为List实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this DataTable dt)
        {
            var mapData = DataReaderHelper.DataTableToDictionary(dt);
            var type = new EntityDynamics().BuildType(typeof(TEntity));
            return (List<TEntity>)InstanceStaticCacheManger.Cache(type, "ToList", (object)mapData);
        }

        /// <summary>
        ///     DataTable转换为数组实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity[] ToArray<TEntity>(this DataTable dt)
        {
            var mapData = DataReaderHelper.DataTableToDictionary(dt);
            var type = new EntityDynamics().BuildType(typeof(TEntity));
            return ((List<TEntity>)InstanceStaticCacheManger.Cache(type, "ToList", (object)mapData)).ToArray();
        }

        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="ds">源DataSet</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this DataSet ds) where T : class, new()
        {
            return ds.Tables.Count == 0 ? null : ds.Tables[0].ToList<T>();
        }

        /// <summary>
        ///     将XML转成实体
        /// </summary>
        public static List<TEntity> ToList<TEntity>(this XElement element) where TEntity : class
        {
            var orm = SetMapCacheManger.Cache(typeof(TEntity));
            var list = new List<TEntity>();

            foreach (var el in element.Elements())
            {
                var t = (TEntity)Activator.CreateInstance(typeof(TEntity));

                //赋值字段
                foreach (var kic in orm.MapList)
                {
                    var type = kic.Key.PropertyType;
                    if (!kic.Key.CanWrite) { continue; }
                    //switch (kic.Value.PropertyExtend)
                    {
                        //case eumPropertyExtend.Attribute:
                        //    if (el.Attribute(kic.Value.Name) == null) { continue; }
                        //kic.Key.SetValue(t, el.Attribute(kic.Value.Name).Value.ConvertType(type), null);
                        //    PropertySetCacheManger.Cache(kic.Key, t, el.Attribute(kic.Value.Name).Value.ConvertType(type));

                        //    break;
                        // case eumPropertyExtend.Element:
                        if (el.Element(kic.Value.Field.Name) == null) { continue; }
                        //kic.Key.SetValue(t, el.Element(kic.Value.Name).Value.ConvertType(type), null);
                        PropertySetCacheManger.Cache(kic.Key, t, el.Element(kic.Value.Field.Name).Value.ConvertType(type));
                        break;
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="lst">集合</param>
        /// <returns></returns>
        public static DataTable ToTable<TEntity>(this List<TEntity> lst) where TEntity : class
        {
            var dt = new DataTable();
            if (lst.Count == 0) { return dt; }
            var map = SetMapCacheManger.Cache(typeof(TEntity));
            var lstFields = map.MapList.Where(o => o.Value.Field.IsMap).OrderBy(o => o.Value.Field.FieldIndex).ToList();
            // 添加DataTable列
            foreach (var field in lstFields)
            {
                // 对   List 类型处理
                var type = field.Key.PropertyType.GetNullableArguments();
                if (type.IsEnum) { type = typeof(int); }
                var col = new DataColumn(field.Value.Field.Name, type) { AutoIncrement = field.Value.Field.IsDbGenerated };
                dt.Columns.Add(col);
            }

            foreach (var info in lst)
            {
                var dr = dt.NewRow();
                foreach (var field in lstFields)
                {
                    var value = EntityHelper.GetValue(info, field.Key.Name, (object)null);
                    var type = field.Key.PropertyType.GetNullableArguments();
                    // 枚举特殊处理
                    if (type.IsEnum) { value = (int)(value ?? 0); }

                    dr[field.Value.Field.Name] = (value == null && !type.IsClass) || field.Value.Field.IsDbGenerated ? 0 : value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}