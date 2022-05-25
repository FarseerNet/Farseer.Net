using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Data.Configuration;
using FS.Data.Inteface;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Data
{
    public class SqlSet : AbsDbSet
    {
        private SqlMapItemConfig _map;

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal SqlSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context: context, pInfo: pInfo);
        }

        /// <summary>
        ///     SQL语句配置文件
        /// </summary>
        protected SqlMapItemConfig Map
        {
            get
            {
                if (_map != null) return _map;
                var name                 = Context.ContextType.FullName + "." + SetMap.TableName;
                var configurationSection = IocManager.GetService<IConfigurationRoot>().GetSection(key: "SqlMap");
                var sqlMapItemConfigs    = configurationSection.GetChildren().Select(selector: o => o.Get<SqlMapItemConfig>()).ToList();
                return _map = sqlMapItemConfigs.Find(match: o => o.Name == name);
            }
        }

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="t"> 失败时返回的值 </param>
        /// <param name="parameters"> 参数 </param>
        public T GetValue<T>(T t = default, params DbParameter[] parameters) => Context.ManualSql.GetValue(sql: Map.Sql, t: t, parameters: parameters);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="t"> 失败时返回的值 </param>
        /// <param name="parameters"> 参数 </param>
        public Task<T> GetValueAsync<T>(T t = default, params DbParameter[] parameters) => Context.ManualSql.GetValueAsync(sql: Map.Sql, t: t, parameters: parameters);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="parameters"> 参数 </param>
        public int Execute(params DbParameter[] parameters) => Context.ManualSql.Execute(sql: Map.Sql, parameters: parameters);

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="parameters"> 参数 </param>
        public Task<int> ExecuteAsync(params DbParameter[] parameters) => Context.ManualSql.ExecuteAsync(sql: Map.Sql, parameters: parameters);
    }
}