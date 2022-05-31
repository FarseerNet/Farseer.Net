using System;
using System.ComponentModel;
using System.Linq;
using Collections.Pooled;
using FS.ElasticSearch.Configuration;
using FS.ElasticSearch.Map;

namespace FS.ElasticSearch.Internal
{
    /// <summary>
    ///     上下文初始化程序
    /// </summary>
    internal class InternalContext : IDisposable
    {
        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="contextType"> 外部上下文类型 </param>
        public InternalContext(Type contextType)
        {
            ContextType = contextType;
            // 上下文映射关系
            ContextMap = new ContextDataMap(type: ContextType);
        }

        /// <summary>
        ///     外部上下文类型
        /// </summary>
        public Type ContextType { get; }

        /// <summary>
        ///     映射结构关系
        /// </summary>
        public ContextDataMap ContextMap { get; set; }

        /// <summary>
        ///     是否初始化实体类名
        /// </summary>
        public bool IsInitModelName { get; internal set; }

        /// <summary>
        ///     初始化数据库环境、实例化子类中，所有Set属性
        /// </summary>
        /// <param name="configName"> </param>
        public void Initializer(string configName)
        {
            // 设置默认的副本、分片、刷新数量
            using var elasticSearchItemConfigs = ElasticSearchConfigRoot.Get().ToPooledList();
            var       esConfig                 = elasticSearchItemConfigs.FirstOrDefault(o => o.Name == configName);
            foreach (var setDataMap in ContextMap.SetDataList)
            {
                setDataMap.SetName(esConfig.ShardsCount, esConfig.ReplicasCount, esConfig.RefreshInterval, esConfig.IndexFormat);
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        [EditorBrowsable(state: EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing"> 是否释放托管资源 </param>
        [EditorBrowsable(state: EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                foreach (var setDataMap in ContextMap.SetDataList)
                {
                    setDataMap.Dispose();
                }
                ContextMap.Dispose();
            }
        }
    }
}