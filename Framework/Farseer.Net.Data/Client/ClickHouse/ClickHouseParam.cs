using System;
using System.Data;
using System.Data.Common;
using FS.Extends;

namespace FS.Data.Client.ClickHouse;

public class ClickHouseParam : AbsDbParam
{
    public ClickHouseParam(DbProviderFactory dbProviderFactory) : base(dbProviderFactory) { }

    public override bool IsSupportParam => false;

    protected override string ParamsPrefix(string paramName) => $"{{{paramName}}}";

    /// <summary>
    ///     创建一个数据库参数对象
    /// </summary>
    /// <param name="columnName">字段名称 </param>
    /// <param name="parameterName"> 参数名称 </param>
    /// <param name="value"> 参数值 </param>
    /// <param name="type"> 参数类型 </param>
    /// <param name="len"> 参数长度 </param>
    /// <param name="output"> 是否是输出值 </param>
    public override DbParameter Create(string columnName, string parameterName, object value, DbType type, bool output = false, int len = 0)
    {
        var dbParam = base.Create(columnName, parameterName: parameterName, value: value, type: type, output: output, len: len);

        switch (type)
        {
            case DbType.String:
                dbParam.ParameterName = $"'{dbParam.Value}'";
                break;
            case DbType.Boolean:
                dbParam.ParameterName = $"{dbParam.Value.ConvertType(defValue: 0)}";
                break;
            case DbType.Date:
            case DbType.DateTime:
            case DbType.DateTime2:
            case DbType.DateTimeOffset:
                dbParam.ParameterName = $"'{(DateTime)dbParam.Value:yyyy-MM-dd HH:mm:ss}'";
                break;
            case DbType.Int16: // 解决ch不支持bool，强制转换成int。在dbType中，bool = int16类型
                dbParam.ParameterName = $"{dbParam.Value.ConvertType(defValue: 0)}";
                break;
            default:
                dbParam.ParameterName = $"{dbParam.Value}";
                break;
        }

        return dbParam;
    }
}