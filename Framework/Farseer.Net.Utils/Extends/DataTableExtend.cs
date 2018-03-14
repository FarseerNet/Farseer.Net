using System.Collections.Generic;
using System.Data;
using System.Linq;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     格式化变量
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     对DataTable排序
        /// </summary>
        /// <param name="dt">要排序的表</param>
        /// <param name="sort">要排序的字段</param>
        public static DataTable Sort(DataTable dt, string sort = "ID DESC")
        {
            var rows = dt.Select("", sort);
            var tmpDt = dt.Clone();

            foreach (var row in rows) { tmpDt.ImportRow(row); }
            return tmpDt;
        }

        /// <summary>
        ///     对DataTable分页
        /// </summary>
        /// <param name="dt">源表</param>
        /// <param name="pageSize">每页显示的记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public static DataTable Split(this DataTable dt, int pageSize = 20, int pageIndex = 1)
        {
            if (pageIndex < 1) { pageIndex = 1; }
            if (pageSize < 1) { pageSize = 1; }
            var dtNew = dt.Clone();

            int firstIndex;

            #region 计算 开始索引

            if (pageIndex == 1) { firstIndex = 0; }
            else
            {
                firstIndex = pageSize * (pageIndex - 1);
                //索引超出记录总数时，返回空的表格
                if (firstIndex > dt.Rows.Count) { return dtNew; }
            }

            #endregion

            #region 计算 结束索引

            var endIndex = pageSize + firstIndex;
            if (endIndex > dt.Rows.Count) { endIndex = dt.Rows.Count; }

            #endregion

            for (var i = firstIndex; i < endIndex; i++) { dtNew.ImportRow(dt.Rows[i]); }
            return dtNew;
        }

        /// <summary>
        ///     DataTable倒序
        /// </summary>
        /// <param name="dt">源DataTable</param>
        public static DataTable Reverse(DataTable dt)
        {
            var tmpDt = dt.Clone();

            for (var i = dt.Rows.Count - 1; i >= 0; i--) { tmpDt.ImportRow(dt.Rows[i]); }
            return tmpDt;
        }

        /// <summary>
        ///     DataTable深度复制
        /// </summary>
        /// <param name="dt">要排序的表</param>
        public static DataTable CloneData(DataTable dt)
        {
            var newTable = dt.Clone();
            dt.Rows.ToRows().ForEach(newTable.ImportRow);
            return newTable;
        }

        /// <summary>
        ///     将DataRowCollection转成List[DataRow]
        /// </summary>
        /// <param name="drc">DataRowCollection</param>
        public static List<DataRow> ToRows(this DataRowCollection drc)
        {
            var lstRow = new List<DataRow>(drc.Count);
            lstRow.AddRange(drc.Cast<DataRow>());
            return lstRow;
        }
    }
}