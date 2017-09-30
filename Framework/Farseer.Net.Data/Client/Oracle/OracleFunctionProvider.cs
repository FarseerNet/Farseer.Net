using Farseer.Net.Data.Infrastructure;

namespace Farseer.Net.Data.Client.Oracle
{
    public class OracleFunctionProvider : AbsFunctionProvider
    {
        public override string CharIndex(string fieldName, string paramName, bool isNot) => $"INSTR({fieldName},{paramName}) {(isNot ? "<=" : ">")} 0";
        public override string StartsWith(string fieldName, string paramName, bool isNot) => $"INSTR({fieldName},{paramName}) {(isNot ? ">" : "=")} 1";
        /// <summary>
        /// 将字段或值转换成短日期
        /// </summary>
        /// <param name="fieldName">字段名称或值</param>
        public override string ToDate(string fieldName) => $"to_date('{fieldName}','yyyy-MM-dd HH24:MI:ss')";
    }
}