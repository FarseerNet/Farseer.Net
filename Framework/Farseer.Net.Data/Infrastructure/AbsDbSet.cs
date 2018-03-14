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
        private PropertyInfo _pInfo;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal InternalContext Context => _dbContext.InternalContext;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        public SetDataMap SetMap => Context.ContextMap.GetEntityMap(_pInfo);

        /// <summary>
        ///     当前队列
        /// </summary>
        internal virtual Queue Queue => Context.QueueManger.CreateQueue(SetMap);

        /// <summary>
        ///     队列管理
        /// </summary>
        internal virtual QueueManger QueueManger => Context.QueueManger;

        /// <summary>
        ///     设置所属上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">当前在上下文中的属性</param>
        internal void SetContext(DbContext context, PropertyInfo pInfo)
        {
            _dbContext = context;
            this._pInfo = pInfo;
        }

        #region 禁用智能提示

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }

        #endregion
    }
}