using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FS.DI;
using FS.ElasticSearch.Cache;
using FS.ElasticSearch.Internal;
using FS.ElasticSearch.Map;
using Nest;

namespace FS.ElasticSearch
{
    public class EsContext : IDisposable
    {
        public IElasticClient Client { get; }

        /// <summary>
        ///     通过配置，连接ElasticSearch
        /// </summary>
        /// <param name="configName">配置名称</param>
        protected EsContext(string configName)
        {
            Client           = IocManager.Instance.Resolve<IElasticClient>($"es_{configName}");
            _internalContext = new InternalContext(this.GetType());

            // 实例化子类中，所有Set属性
            ContextSetTypeCacheManger.Cache(this.GetType()).Item2(this);
        }

        /// <summary>
        ///     静态实例
        /// </summary>
        internal static TPo Data<TPo>() where TPo : EsContext, new() => new();

        /// <summary>
        ///     在创建模型时调用
        /// </summary>
        protected virtual void CreateModelInit(Dictionary<string, SetDataMap> map)
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
                if (!_internalContext.IsInitializer)
                {
                    _internalContext.Initializer();
                }

                if (!_internalContext.IsInitModelName)
                {
                    // 初始化模型映射
                    CreateModelInit(_internalContext.ContextMap.SetDataList.ToDictionary(o => o.ClassProperty.Name));
                    _internalContext.IsInitModelName = true;
                }

                return _internalContext;
            }
        }

        #endregion

        #region 动态查找Set类型
        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public IndexSet<TEntity> IndexSet<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(IndexSet<TEntity>), propertyName);
            return new IndexSet<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public IndexSet<TEntity> IndexSet<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new IndexSet<TEntity>(this, propertyInfo);
        }
        

        /// <summary>
        ///     动态返回Set类型的属性数据
        /// </summary>
        /// <param name="setType">Set的类型</param>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        private PropertyInfo GetSetPropertyInfo(Type setType, string propertyName = null)
        {
            var lstPropertyInfo = this.GetType().GetProperties();
            var lst             = lstPropertyInfo.Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.PropertyType == setType).Where(propertyInfo => propertyName == null || propertyInfo.Name == propertyName);
            if (lst == null) { throw new Exception("未找到当前类型的Set属性：" + setType.GetGenericArguments()[0]); }
            if (lst.Count() > 1) { throw new Exception("找到多个Set属性，请指定propertyName确定唯一。：" + setType.GetGenericArguments()[0]); }
            return lst.FirstOrDefault();
        }
        
        #endregion
        
        public void Dispose()
        {
        }
    }
}