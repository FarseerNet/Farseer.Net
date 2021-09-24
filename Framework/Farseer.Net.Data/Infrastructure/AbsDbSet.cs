using System;
using System.ComponentModel;
using System.Reflection;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Infrastructure
{
    /// <summary>
    ///     Set基类
    /// </summary>
    public abstract class AbsDbSet
    {
        /// <summary>
        ///     数据库上下文
        /// </summary>
        private DbContext _dbContext;

        /// <summary>
        ///     当前在上下文中的属性
        /// </summary>
        private PropertyInfo _setPropertyInfo;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal InternalContext Context => _dbContext.InternalContext;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        public SetDataMap SetMap => Context.ContextMap.GetEntityMap(setPropertyInfo: _setPropertyInfo);

        /// <summary>
        ///     当前队列
        /// </summary>
        internal virtual Queue Queue => Context.QueueManger.CreateQueue(map: SetMap);

        /// <summary>
        ///     队列管理
        /// </summary>
        internal virtual QueueManger QueueManger => Context.QueueManger;

        /// <summary>
        ///     设置所属上下文
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 当前在上下文中的属性 </param>
        internal void SetContext(DbContext context, PropertyInfo pInfo)
        {
            _dbContext       = context;
            _setPropertyInfo = pInfo;
        }

        #region 禁用智能提示

        [EditorBrowsable(state: EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj: obj);

        [EditorBrowsable(state: EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(state: EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();

        [EditorBrowsable(state: EditorBrowsableState.Never)]
        public new Type GetType() => base.GetType();

        #endregion
    }
}