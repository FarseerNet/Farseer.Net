using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FS.DI;
using FS.ElasticSearch.Internal;
using FS.ElasticSearch.Map;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace FS.ElasticSearch
{
    /// <summary>
    /// 索引操作
    /// </summary>
    public class IndexSet<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// 缓存是否包含索引
        /// </summary>
        private static readonly Dictionary<string, bool> IndexCache = new();

        private static object objLock = new();

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private EsContext _esContext;

        /// <summary>
        ///     当前在上下文中的属性
        /// </summary>
        private PropertyInfo _pInfo;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal InternalContext Context => _esContext.InternalContext;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        public SetDataMap SetMap => Context.ContextMap.GetEntityMap(_pInfo);

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal IndexSet(EsContext context, PropertyInfo pInfo)
        {
            SetContext(context, pInfo);
            Client = _esContext.Client;
            WhenNotExistsAddIndex();
        }

        /// <summary>
        /// ES客户端
        /// </summary>
        public IElasticClient Client { get; }

        /// <summary>
        /// 写入数据
        /// </summary>
        public virtual bool Insert(TEntity model)
        {
            var result = Client.Index(new IndexRequest<TEntity>(model, SetMap.IndexName));
            if (!result.IsValid)
            {
                IocManager.Instance.Logger<IndexSet<TEntity>>().LogError($"索引失败：{JsonConvert.SerializeObject(model)} \r\n" + result.OriginalException.Message);
            }

            return result.IsValid;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public virtual async Task<bool> InsertAsync(TEntity model)
        {
            var result = await Client.IndexAsync(new IndexRequest<TEntity>(model, SetMap.IndexName));
            if (!result.IsValid)
            {
                IocManager.Instance.Logger<IndexSet<TEntity>>().LogError($"索引失败：{JsonConvert.SerializeObject(model)} \r\n" + result.OriginalException.Message);
            }

            return result.IsValid;
        }

        /// <summary>
        /// 索引不存在时，创建索引
        /// </summary>
        protected void WhenNotExistsAddIndex()
        {
            if (!IndexCache.ContainsKey(SetMap.IndexName) || !IndexCache[SetMap.IndexName])
            {
                lock (objLock)
                {
                    if (!Client.Indices.Exists(SetMap.IndexName).Exists)
                    {
                        IndexCache[SetMap.IndexName] = CreateIndex();
                    }
                }
            }
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        protected bool CreateIndex()
        {
            var rsp = Client.Indices.Create(SetMap.IndexName, c => c
                .Map<TEntity>(m => m.AutoMap())
                .Aliases(des =>
                {
                    foreach (var aliasName in SetMap.AliasNames)
                    {
                        des.Alias(aliasName);
                    }

                    return des;
                }).Settings(s => s.NumberOfReplicas(SetMap.ReplicasCount).NumberOfShards(SetMap.ShardsCount))
            );
            if (!rsp.IsValid) throw new Exception(rsp.OriginalException.Message);

            return true;
        }

        /// <summary>
        ///     设置所属上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">当前在上下文中的属性</param>
        internal void SetContext(EsContext context, PropertyInfo pInfo)
        {
            _esContext  = context;
            this._pInfo = pInfo;
        }
    }
}