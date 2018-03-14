using System.Collections.Generic;
using FS.Utils.Common;

namespace FS.Data.Infrastructure
{
    /// <summary>
    /// 数据库函数提供者（不同数据库的函数）
    /// </summary>
    public abstract class AbsFunctionProvider
    {
        /// <summary>
        /// 搜索字符串是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="paramName">参数化名称（也可以是字段值）</param>
        /// <param name="isNot">true：不存在</param>
        public virtual string CharIndex(string fieldName, string paramName, bool isNot) => $"CHARINDEX({paramName},{fieldName}) {(isNot ? "<=" : ">")} 0";
        /// <summary>
        /// 搜索字符串是否在开始位置
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="paramName">参数化名称（也可以是字段值）</param>
        /// <param name="isNot">true：不存在</param>
        public virtual string StartsWith(string fieldName, string paramName, bool isNot) => $"CHARINDEX({paramName},{fieldName}) {(isNot ? ">" : "=")} 1";
        /// <summary>
        /// 搜索字符串是否在结束位置
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="paramName">参数化名称（也可以是字段值）</param>
        /// <param name="isNot">true：不存在</param>
        public virtual string EndsWith(string fieldName, string paramName, bool isNot) => $"{fieldName} {(isNot ? "Not" : "")} LIKE {paramName}";
        /// <summary>
        /// 忽略大小写判断字符串是否相等
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="paramName">参数化名称（也可以是字段值）</param>
        /// <param name="isNot">true：不存在</param>
        public virtual string IsEquals(string fieldName, string paramName, bool isNot) => $"{fieldName} {(isNot ? "<>" : "=")} {paramName}";
        /// <summary>
        /// 忽略大小写判断字符串是否相等
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="paramName">参数化名称（也可以是字段值）</param>
        /// <param name="isNot">true：不存在</param>
        public virtual string Equals(string fieldName, string paramName, bool isNot) => $"{fieldName} {(isNot ? "<>" : "=")} {paramName}";
        /// <summary>
        /// 将字段或值转换成短日期
        /// </summary>
        /// <param name="fieldName">字段名称或值</param>
        public virtual string ToShortDate(string fieldName) => $"CONVERT(varchar(100), {fieldName}, 23)";
        /// <summary>
        /// 包含的数字值是否存在
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="lstParamName">多个值</param>
        /// <param name="isNot">true：不存在</param>
        public virtual string In(string fieldName, List<string> lstParamName, bool isNot) => $"{fieldName} {(isNot ? "Not" : "")} IN ({IEnumerableHelper.ToString(lstParamName)})";
        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="fieldName">字段名称或值</param>
        public virtual string Len(string fieldName) => $"LEN({fieldName})";
        /// <summary>
        /// 将字段或值转换成短日期
        /// </summary>
        /// <param name="fieldName">字段名称或值</param>
        public virtual string ToDate(string fieldName) => $"CONVERT(varchar(100), {fieldName}, 20)";

        public virtual string GetColumns(string tableName) => "";
    }
}