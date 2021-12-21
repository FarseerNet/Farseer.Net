using System;
using FS.Cache;
using FS.Data.Features;

namespace FS.Data.Cache
{
    /// <summary>
    ///     创建逻辑删除功能
    /// </summary>
    public class SortDeleteCacheManger : AbsCacheManger<int, SortDelete>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private readonly Type _entityType;

        private readonly EumSortDeleteType _field;
        private readonly string            _name;
        private readonly object            _value;

        /// <summary>
        ///     创建逻辑删除功能
        /// </summary>
        /// <param name="name"> 软删除标记字段名称 </param>
        /// <param name="field"> 数据库字段类型 </param>
        /// <param name="value"> 标记值 </param>
        /// <param name="entityType"> 实体类型 </param>
        private SortDeleteCacheManger(string name, EumSortDeleteType field, object value, Type entityType)
        {
            _field      = field;
            _value      = value;
            _entityType = entityType;
            _name       = name;

            Key = name.GetHashCode() + field.GetHashCode();
            if (value != null) Key += value.GetHashCode();
            Key += entityType.GetHashCode();
        }

        protected override SortDelete SetCacheLock()
        {
            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];

            lock (LockObject)
            {
                if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
                //缓存中没有找到，新建一个实例
                var sortDelete = new SortDelete { Name = _name, FieldType = _field, Value = _value };
                sortDelete.Init(entityType: _entityType);

                return CacheList[key: Key] = sortDelete;
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="name"> 软删除标记字段名称 </param>
        /// <param name="field"> 数据库字段类型 </param>
        /// <param name="value"> 标记值 </param>
        /// <param name="entityType"> 实体类型 </param>
        public static SortDelete Cache(string name, EumSortDeleteType field, object value, Type entityType) => new SortDeleteCacheManger(name: name, field: field, value: value, entityType: entityType).GetValue();
    }
}