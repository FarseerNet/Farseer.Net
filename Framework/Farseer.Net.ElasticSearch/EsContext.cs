using System;
using System.Linq;
using System.Reflection;
using FS.DI;
using FS.ElasticSearch.Cache;
using FS.ElasticSearch.Internal;
using Nest;

namespace FS.ElasticSearch
{
    public class EsContext : IDisposable
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        private readonly string _configName;

        /// <summary>
        ///     通过配置，连接ElasticSearch
        /// </summary>
        /// <param name="configName"> 配置名称 </param>
        protected EsContext(string configName)
        {
            InternalContext = new InternalContext(contextType: GetType());
            _configName     = configName;
            Client          = IocManager.GetService<IElasticClient>(name: configName);

            InitializerInternalContext();
            InitializerSet();
        }

        public IElasticClient Client { get; }

        /// <summary>
        ///     初始化内部InternalContext
        /// </summary>
        private void InitializerInternalContext()
        {
            InternalContext.Initializer(_configName);
        }

        /// <summary>
        ///     不初始化ContextConnection（用于动态改变数据库连接方式）
        /// </summary>
        private void InitializerSet()
        {
            // 实例化子类中，所有Set属性
            ContextSetTypeCacheManger.Cache(contextKey: GetType()).Item2(obj: this);

            // 设置表名称
            CreateModelInit();
        }

        /// <summary>
        ///     静态实例
        /// </summary>
        internal static TPo Data<TPo>() where TPo : EsContext, new() => new();

        /// <summary>
        ///     在创建模型时调用
        /// </summary>
        protected virtual void CreateModelInit()
        {
        }

        #region DbContextInitializer上下文初始化器

        /// <summary>
        ///     上下文初始化器
        /// </summary>
        internal InternalContext InternalContext { get; }

        #endregion

        #region 动态查找Set类型

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName"> 当有多个相同类型TEntity时，须使用propertyName来寻找唯一 </param>
        /// <typeparam name="TEntity"> </typeparam>
        public IndexSet<TEntity> IndexSet<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(setType: typeof(IndexSet<TEntity>), propertyName: propertyName);
            return new IndexSet<TEntity>(context: this, pInfo: pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo"> 上下文中的属性 </param>
        /// <typeparam name="TEntity"> </typeparam>
        public IndexSet<TEntity> IndexSet<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new() => new(context: this, pInfo: propertyInfo);

        /// <summary>
        ///     动态返回Set类型的属性数据
        /// </summary>
        /// <param name="setType"> Set的类型 </param>
        /// <param name="propertyName"> 当有多个相同类型TEntity时，须使用propertyName来寻找唯一 </param>
        private PropertyInfo GetSetPropertyInfo(Type setType, string propertyName = null)
        {
            var lstPropertyInfo = GetType().GetProperties();
            var lst             = lstPropertyInfo.Where(predicate: propertyInfo => propertyInfo.CanWrite && propertyInfo.PropertyType == setType).Where(predicate: propertyInfo => propertyName == null || propertyInfo.Name == propertyName);
            if (lst         == null) throw new Exception(message: "未找到当前类型的Set属性："              + setType.GetGenericArguments()[0]);
            if (lst.Count() > 1) throw new Exception(message: "找到多个Set属性，请指定propertyName确定唯一。：" + setType.GetGenericArguments()[0]);
            return lst.FirstOrDefault();
        }

        #endregion

        public void Dispose()
        {
            InternalContext.Dispose();
        }
    }
}