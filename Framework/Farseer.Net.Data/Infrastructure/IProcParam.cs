using System.Collections.Generic;
using System.Data.Common;

namespace FS.Data.Infrastructure
{
    /// <summary>
    ///     SQL参数
    /// </summary>
    public interface IProcParam
    {
        /// <summary>
        ///     数据库名称
        /// </summary>
        string DbName { get; }

        /// <summary>
        ///     存储过程名
        /// </summary>
        string ProcName { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; }
    }
}