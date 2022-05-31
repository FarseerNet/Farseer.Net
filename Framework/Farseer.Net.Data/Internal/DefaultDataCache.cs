using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FS.Data.Cache;
using FS.Data.Inteface;

namespace FS.Data.Internal
{
    /// <summary>
    ///     默认的表、视图数据缓存处理方式（不支持分布式）
    /// </summary>
    public class DefaultDataCache<TEntity> : IDataCache<TEntity> where TEntity : class, new()
    {
        private readonly AbsDbSet _set;

        public DefaultDataCache(AbsDbSet set)
        {
            _set = set;
        }

        public List<TEntity> Get()
        {
            return EntityCacheManger.Cache<TEntity>(key: _set.SetMap.PhysicsMap, initCache: () =>
            {
                using var expBuilder = new ExpressionBuilder(map: _set.SetMap);
                expBuilder.DeleteSortCondition();
                return _set.Context.ExecuteSql.ToList<TEntity>(callMethod: $"{typeof(TEntity).Name}.Get", sqlParam: _set.Context.DbProvider.CreateSqlBuilder(expBuilder: expBuilder, _set.SetMap).ToList()).ToList();
            });
        }

        public void Update(IEnumerable<TEntity> lst)
        {
            EntityCacheManger.Update(key: _set.SetMap.PhysicsMap, value: (IList)lst);
        }
    }
}