using System.Collections.Generic;
using System.Reflection;
using FS.Data.Inteface;
using FS.Data.Internal;

namespace FS.Data
{
    /// <summary>
    ///     整表数据缓存Set
    /// </summary>
    /// <typeparam name="TEntity"> </typeparam>
    public class ViewSetCache<TEntity> : IDbSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     数据缓存操作接口
        /// </summary>
        private readonly IDataCache<TEntity> _dataCache;

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal ViewSetCache(DbContext context, PropertyInfo pInfo)
        {
            var set = new ViewSet<TEntity>(context: context, pInfo: pInfo);
            _dataCache = new DefaultDataCache<TEntity>(set: set);
        }

        /// <summary>
        ///     当前缓存
        /// </summary>
        public List<TEntity> Cache => _dataCache.Get();
    }
}