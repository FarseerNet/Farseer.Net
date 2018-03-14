using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using FS.Configuration;
using FS.Data.Configuration;
using FS.Data.Infrastructure;
using FS.DI;
using FS.Configuration;

namespace FS.Data
{
    public class SqlSet : AbsDbSet
    {
        private SqlMapItemConfig _map;

        /// <summary>
        ///     SQL语句配置文件
        /// </summary>
        protected SqlMapItemConfig Map
        {
            get
            {
                if (_map != null) return _map;
                var name = Context.ContextType.FullName + "." + SetMap.Name;
                return _map = IocManager.Instance.Resolve<IConfigResolver>().SqlMapConfig().Items.Find(o=>o.Name==name);
            }
        }

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal SqlSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context, pInfo);
        }

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="t">失败时返回的值</param>
        /// <param name="parameters">参数</param>
        public T GetValue<T>(T t = default(T), params DbParameter[] parameters) => Context.ManualSql.GetValue(Map.Sql, t, parameters);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="t">失败时返回的值</param>
        /// <param name="parameters">参数</param>
        public Task<T> GetValueAsync<T>(T t = default(T), params DbParameter[] parameters) => Context.ManualSql.GetValueAsync(Map.Sql, t, parameters);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="parameters">参数</param>
        public int Execute(params DbParameter[] parameters) => Context.ManualSql.Execute(Map.Sql, parameters);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="parameters">参数</param>
        public Task<int> ExecuteAsync(params DbParameter[] parameters) => Context.ManualSql.ExecuteAsync(Map.Sql, parameters);
    }
}