using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Infrastructure
{
    /// <summary>
    /// 数据缓存操作接口
    /// </summary>
    public interface IDataCache<TEntity>  where TEntity : class, new()
    {
        /// <summary>
        ///     从缓存中读取，并返回数据
        /// </summary>
        IEnumerable<TEntity> Get();

        /// <summary>
        ///     将数据更新到缓存中
        /// </summary>
        void Update(IEnumerable<TEntity> lst);
    }
}
