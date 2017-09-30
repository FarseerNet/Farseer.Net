using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Farseer.Net.Data.Infrastructure
{
    /// <summary>
    ///     SQL语句与参数
    /// </summary>
    public interface ISqlParam : IDisposable
    {
        /// <summary>
        ///     表名/视图名
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     当前生成的SQL语句
        /// </summary>
        StringBuilder Sql { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        List<DbParameter> Param { get; }
    }
}