using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Collections.Pooled;
using FS.DI;
using FS.ElasticSearch.Cache;
using FS.ElasticSearch.Configuration;
using FS.ElasticSearch.Internal;
using FS.ElasticSearch.Map;
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
            _configName      = configName;
            Client           = IocManager.GetService<IElasticClient>(name: configName);
            _internalContext = new InternalContext(contextType: GetType());

            // 实例化子类中，所有Set属性
            ContextSetTypeCacheManger.Cache(contextKey: GetType()).Item2(obj: this);
        }

        public IElasticClient Client { get; }

        public void Dispose()
        {
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
        private readonly InternalContext _internalContext;

        /// <summary>
        ///     上下文初始化器
        /// </summary>
        internal InternalContext InternalContext
        {
            get
            {
                if (!_internalContext.IsInitializer) _internalContext.Initializer();

                if (!_internalContext.IsInitModelName)
                {
                    // 设置默认的副本、分片、刷新数量
                    using var elasticSearchItemConfigs = ElasticSearchConfigRoot.Get().ToPooledList();
                    var       esConfig                 = elasticSearchItemConfigs.FirstOrDefault(o => o.Name == _configName);
                    foreach (var setDataMap in _internalContext.ContextMap.SetDataList)
                    {
                        setDataMap.SetName(esConfig.ShardsCount, esConfig.ReplicasCount, esConfig.RefreshInterval, esConfig.IndexFormat);
                    }

                    _internalContext.IsInitModelName = true;
                    // 初始化模型映射
                    CreateModelInit();
                }

                return _internalContext;
            }
        }

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
    }
}