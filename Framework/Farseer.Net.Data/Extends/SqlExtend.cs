using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS.Data.Inteface;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="isContainsSub"> 是否获取子节点 </param>
        /// <param name="ID"> 上级ID </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<int> GetSubIDList<TEntity>(this IEnumerable<TEntity> lstCate, int? ID, bool isContainsSub = true, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            using var lst = lstCate.GetSubList(ID, isContainsSub, isAddMySelf);
            return lst?.Select(o => o.ID.GetValueOrDefault()).ToPooledList() ?? new PooledList<int>();
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="isContainsSub"> 是否获取子节点 </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<int> GetSubIDList<TEntity>(this IEnumerable<TEntity> lstCate, string caption, bool isContainsSub = true, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            using var lst = lstCate.GetSubList(caption, isContainsSub, isAddMySelf);
            return lst?.Select(o => o.ID.GetValueOrDefault()).ToPooledList() ?? new PooledList<int>();
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="ID"> 上级ID </param>
        /// <param name="isContainsSub"> 是否获取子节点 </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<TEntity> GetSubList<TEntity>(this IEnumerable<TEntity> lstCate, int? ID, bool isContainsSub = true, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            var lst = new PooledList<TEntity>();
            if (isAddMySelf)
            {
                var info = lstCate.FirstOrDefault(o => o.ID == ID);
                if (info != null)
                {
                    lst.Add(info);
                }
            }

            foreach (var info in lstCate.Where(o => o.ParentID == ID).ToList())
            {
                lst.Add(info);
                if (!isContainsSub)
                {
                    continue;
                }

                lst.AddRange(lstCate.GetSubList(info.ID, isContainsSub, false));
            }

            return lst;
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="isContainsSub"> 是否获取子节点 </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<TEntity> GetSubList<TEntity>(this IEnumerable<TEntity> lstCate, string caption, bool isContainsSub = true, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? new PooledList<TEntity>() : lstCate.GetSubList(info.ID, isContainsSub, isAddMySelf);
        }

        /// <summary>
        ///     通过标题，获取分类数据
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static int GetID<TEntity>(this IEnumerable<TEntity> lstCate, string caption) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetInfo(caption);
            return info?.ID.GetValueOrDefault() ?? 0;
        }

        /// <summary>
        ///     通过标题，获取分类数据
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static TEntity GetInfo<TEntity>(this IEnumerable<TEntity> lstCate, string caption) where TEntity : class, ICate, new()
        {
            var info = lstCate.FirstOrDefault(o => StringHelper.IsEquals(o.Caption, caption));
            return info;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="ID"> 当前分类数据ID </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static int GetFirstID<TEntity>(this IEnumerable<TEntity> lstCate, int? ID) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetFirstInfo(ID);
            return info?.ID.GetValueOrDefault() ?? 0;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="ID"> 当前分类数据ID </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static TEntity GetFirstInfo<TEntity>(this IEnumerable<TEntity> lstCate, int? ID) where TEntity : class, ICate, new()
        {
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info == null)
            {
                return null;
            }

            if (lstCate.Count(o => o.ID == info.ParentID) > 0)
            {
                info = lstCate.GetFirstInfo(info.ParentID);
            }

            return info;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static int GetFirstID<TEntity>(this IEnumerable<TEntity> lstCate, string caption) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetFirstInfo(caption);
            return info?.ID.GetValueOrDefault() ?? 0;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static TEntity GetFirstInfo<TEntity>(this IEnumerable<TEntity> lstCate, string caption) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? null : lstCate.GetFirstInfo(info.ParentID);
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="ID"> 当前分类数据ID </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static int GetParentID<TEntity>(this IEnumerable<TEntity> lstCate, int? ID) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetParentInfo(ID);
            return info?.ID.GetValueOrDefault() ?? 0;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="ID"> 当前分类数据ID </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static TEntity GetParentInfo<TEntity>(this IEnumerable<TEntity> lstCate, int? ID) where TEntity : class, ICate, new()
        {
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info != null)
            {
                info = lstCate.FirstOrDefault(o => o.ID == info.ParentID);
            }

            return info;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static int GetParentID<TEntity>(this IEnumerable<TEntity> lstCate, string caption) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetParentInfo(caption);
            return info?.ID.GetValueOrDefault() ?? 0;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static TEntity GetParentInfo<TEntity>(this IEnumerable<TEntity> lstCate, string caption) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? null : lstCate.GetParentInfo(info.ID);
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="ID"> 当前分类数据ID </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<int> GetParentIDList<TEntity>(this IEnumerable<TEntity> lstCate, int? ID, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            using var lst = lstCate.GetParentList(ID, isAddMySelf);
            return lst?.Select(o => o.ID.GetValueOrDefault()).ToPooledList() ?? new PooledList<int>();
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="ID"> 当前分类数据ID </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<TEntity> GetParentList<TEntity>(this IEnumerable<TEntity> lstCate, int? ID, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            var lst  = new PooledList<TEntity>();
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info == null)
            {
                return lst;
            }

            lst.AddRange(lstCate.GetParentList(info.ParentID, true));
            if (isAddMySelf)
            {
                lst.Add(info);
            }

            return lst;
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<int> GetParentIDList<TEntity>(this IEnumerable<TEntity> lstCate, string caption, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            using var lst = lstCate.GetParentList(caption, isAddMySelf);
            return lst?.Select(o => o.ID.GetValueOrDefault()).ToPooledList() ?? new PooledList<int>();
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="caption"> 分类标题 </param>
        /// <param name="isAddMySelf"> 是否添加自己 </param>
        /// <param name="lstCate"> 分类列表 </param>
        public static PooledList<TEntity> GetParentList<TEntity>(this IEnumerable<TEntity> lstCate, string caption, bool isAddMySelf = false) where TEntity : class, ICate, new()
        {
            var info = lstCate.GetInfo(caption);
            return info == null ? new PooledList<TEntity>() : lstCate.GetParentList(info.ID, isAddMySelf);
        }
    }
}