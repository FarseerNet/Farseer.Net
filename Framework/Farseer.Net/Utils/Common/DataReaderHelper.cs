// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-02 18:11
// ********************************************

using System.Collections.Generic;
using System.Data;
using FS.Core;

namespace FS.Utils.Common
{
    /// <summary>
    ///     DataReader帮助器
    /// </summary>
    public class DataReaderHelper
    {
        /// <summary>
        ///     IDataReader转换字典
        /// </summary>
        public static MapingData[] DataReaderToDictionary(IDataReader reader)
        {
            // 获取数据的列并转成字典
            var dicColumns = new MapingData[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var mapData = dicColumns[i] = new MapingData();
                mapData.ColumnName = reader.GetName(i: i);
                mapData.DataList   = new List<object>();
            }

            // 遍历行
            while (reader.Read())
            {
                // 当前记录的所有字段的值
                var arrVals = new object[reader.FieldCount];
                reader.GetValues(values: arrVals);
                // 遍历列
                for (var i = 0; i < dicColumns.Length; i++) dicColumns[i].DataList.Add(item: arrVals[i]);
            }

            reader.Close();
            return dicColumns;
        }

        /// <summary>
        ///     DataTable转字典
        /// </summary>
        public static MapingData[] DataTableToDictionary(DataTable dt)
        {
            // 获取数据的列并转成字典
            var cols       = dt.Columns;
            var dicColumns = new MapingData[cols.Count];
            for (var i = 0; i < cols.Count; i++)
            {
                var mapData = dicColumns[i] = new MapingData();
                mapData.ColumnName = cols[index: i].ColumnName;
                mapData.DataList   = new List<object>(capacity: dt.Rows.Count);
            }

            // DataRowCollection转成DataRow[]
            var rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(array: rows, index: 0);

            // 遍历DataRow
            foreach (var dr in rows)
            {
                var arrVals = dr.ItemArray;
                // 遍历列
                for (var i = 0; i < dicColumns.Length; i++) dicColumns[i].DataList.Add(item: arrVals[i]);
            }

            return dicColumns;
        }
    }
}