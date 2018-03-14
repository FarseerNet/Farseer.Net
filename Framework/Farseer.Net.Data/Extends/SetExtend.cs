using FS.Core;
using FS.Data;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToNextEntity<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, IEntity, new()
        {
            return ts.Where(o => o.ID > ID).Asc(o => o.ID).ToEntity();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToPreviousEntity<TEntity>(this TableSet<TEntity> ts, int? ID) where TEntity : class, IEntity, new()
        {
            return ts.Where(o => o.ID < ID).Desc(o => o.ID).ToEntity();
        }

        /// <summary>
        ///     获取下一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToNextEntity<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, IEntity, new()
        {
            return ts.Where(o => o.ID > ID).Asc(o => o.ID).ToEntity();
        }

        /// <summary>
        ///     获取上一条记录
        /// </summary>
        /// <param name="ID">当前ID</param>
        /// <param name="ts">TableSet</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToPreviousEntity<TEntity>(this ViewSet<TEntity> ts, int? ID) where TEntity : class, IEntity, new()
        {
            return ts.Where(o => o.ID < ID).Desc(o => o.ID).ToEntity();
        }
    }
}