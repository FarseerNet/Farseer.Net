using System;
using Farseer.Net.Cache;
using Farseer.Net.Data.Features;

namespace Farseer.Net.Data.Cache
{
    /// <summary>
    ///     创建逻辑删除功能
    /// </summary>
    public class SortDeleteCacheManger : AbsCacheManger<int, SortDelete>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private readonly eumSortDeleteType _field;
        private readonly object _value;
        private readonly Type _entityType;
        private readonly string _name;

        /// <summary>
        ///     创建逻辑删除功能
        /// </summary>
        /// <param name="name">软删除标记字段名称</param>
        /// <param name="field">数据库字段类型</param>
        /// <param name="value">标记值</param>
        /// <param name="entityType">实体类型</param>
        private SortDeleteCacheManger(string name, eumSortDeleteType field, object value, Type entityType)
        {
            this._field = field;
            this._value = value;
            _entityType = entityType;
            _name = name;

            Key = name.GetHashCode() + field.GetHashCode();
            if (value != null) { Key += value.GetHashCode(); }
            Key += entityType.GetHashCode();
        }

        protected override SortDelete SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                //缓存中没有找到，新建一个实例
                var sortDelete = new SortDelete { Name = _name, FieldType = _field, Value = _value };
                sortDelete.Init(_entityType);

                return (CacheList[Key] = sortDelete);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="name">软删除标记字段名称</param>
        /// <param name="field">数据库字段类型</param>
        /// <param name="value">标记值</param>
        /// <param name="entityType">实体类型</param>
        public static SortDelete Cache(string name, eumSortDeleteType field, object value, Type entityType)
        {
            return new SortDeleteCacheManger(name, field, value, entityType).GetValue();
        }
    }
}