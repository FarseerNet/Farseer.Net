using System.Collections.Generic;
using System.Data.Common;
using FS.Data.Map;

namespace FS.Data.Inteface
{
    /// <summary>
    ///     SQL参数
    /// </summary>
    public interface IProcParam
    {
        /// <summary>
        /// 实体类结构映射
        /// </summary>
        public SetDataMap SetMap { get; }

        /// <summary>
        ///     存储过程名
        /// </summary>
        string ProcName { get; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        IEnumerable<DbParameter> Param { get; }
    }
}