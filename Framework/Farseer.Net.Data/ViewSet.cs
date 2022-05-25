using System.Reflection;
using FS.Data.Inteface;

namespace FS.Data
{
    /// <summary>
    ///     视图操作
    /// </summary>
    /// <typeparam name="TEntity"> 实体 </typeparam>
    public sealed class ViewSet<TEntity> : ReadDbSet<ViewSet<TEntity>, TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal ViewSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context: context, pInfo: pInfo);
        }
    }
}