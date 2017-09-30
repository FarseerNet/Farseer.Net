using System.Collections.Generic;
using System.Data.Common;

namespace Farseer.Net.Data.Infrastructure
{
    /// <summary>
    ///     SQL参数
    /// </summary>
    public interface IProcParam
    {
        /// <summary>
        ///     存储过程名
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; }
    }
}