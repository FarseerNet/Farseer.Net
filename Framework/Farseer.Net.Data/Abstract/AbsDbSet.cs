using System;
using System.ComponentModel;
using System.Reflection;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Abstract
{
    /// <summary>
    ///     Set基类
    /// </summary>
    public abstract class AbsDbSet
    {
        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal InternalContext Context { get; private set; }

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        public SetDataMap SetMap { get; private set; }

        /// <summary>
        ///     当前队列
        /// </summary>
        internal virtual Query Query => Context.QueryManger.CreateQueue(map: SetMap);

        /// <summary>
        ///     队列管理
        /// </summary>
        internal virtual QueryManger QueryManger => Context.QueryManger;

        /// <summary>
        ///     设置所属上下文
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 当前在上下文中的属性 </param>
        internal void SetContext(DbContext context, PropertyInfo pInfo)
        {
            Context    = context.InternalContext;
            SetMap     = context.ContextMap.GetEntityMap(pInfo);
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