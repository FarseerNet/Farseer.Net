using System;
using System.ComponentModel;
using System.Linq;
using Elasticsearch.Net;
using FS.ElasticSearch.Map;
using Nest;

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
        /// <param name="contextType">外部上下文类型</param>
        public InternalContext(Type contextType)
        {
            this.ContextType = contextType;
            // 上下文映射关系
            this.ContextMap = new ContextDataMap(ContextType);
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
        ///     是否初始化
        /// </summary>
        public bool IsInitializer { get; private set; }

        /// <summary>
        ///     是否初始化实体类名
        /// </summary>
        public bool IsInitModelName { get; internal set; }

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     初始化数据库环境、实例化子类中，所有Set属性
        /// </summary>
        public void Initializer()
        {
            if (IsInitializer) { return; }

            // 上下文映射关系
            this.ContextMap = new ContextDataMap(ContextType);
            
            IsInitializer = true;
        }
    }
}