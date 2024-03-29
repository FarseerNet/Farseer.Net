using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Collections.Pooled;
using FS.Cache;
using FS.Core.Abstract.Data;
using FS.Core.AOP.LinkTrack;
using FS.Core.LinkTrack;
using FS.DI;
using FS.ElasticSearch.ExpressionVisitor;
using FS.ElasticSearch.Internal;
using FS.ElasticSearch.Map;
using FS.Extends;
using Microsoft.Extensions.Logging;
using Nest;

namespace FS.ElasticSearch
{
    /// <summary>
    ///     索引操作
    /// </summary>
    public class IndexSet<TDocument> where TDocument : class, new()
    {
        /// <summary>
        ///     缓存是否包含索引
        /// </summary>
        private static readonly PooledDictionary<string, bool> IndexCache = new();

        private static readonly object objLock = new();

        /// <summary>
        ///     条件语句
        /// </summary>
        private QueryContainer _query = new();

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private EsContext _esContext;

        /// <summary>
        ///     当前在上下文中的属性
        /// </summary>
        private PropertyInfo _pInfo;

        /// <summary>
        ///     筛选字段
        /// </summary>
        private Expression<Func<TDocument, object>>[] _selectFields;

        /// <summary>
        ///     排序语句
        /// </summary>
        private Func<SortDescriptor<TDocument>, IPromise<IList<ISort>>> _sort;

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 属性类型 </param>
        internal IndexSet(EsContext context, PropertyInfo pInfo)
        {
            SetContext(context: context, pInfo: pInfo);
            Client = _esContext.Client;
        }

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal InternalContext Context => _esContext.InternalContext;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        public SetDataMap SetMap => Context.ContextMap.GetEntityMap(setPropertyInfo: _pInfo);

        /// <summary>
        ///     ES客户端
        /// </summary>
        public IElasticClient Client { get; }

        /// <summary>
        ///     动态设置索引名称、别名
        /// </summary>
        /// <param name="indexName"> 新的索引名称 </param>
        /// <param name="shardsCount"> 新的分片数量 </param>
        /// <param name="replicasCount"> 新的副本数量 </param>
        /// <param name="aliasNames"> 别名 </param>
        /// <param name="refreshInterval">刷新间隔 </param>
        public IndexSet<TDocument> SetName(string indexName, int shardsCount = 3, int replicasCount = 1, int refreshInterval = 1, params string[] aliasNames)
        {
            SetMap.SetName(indexName: indexName, shardsCount: shardsCount, replicasCount: replicasCount, refreshInterval, aliasNames: aliasNames);
            return this;
        }

        /// <summary>
        ///     动态设置索引名称、别名
        /// </summary>
        /// <param name="indexName"> 新的索引名称 </param>
        public IndexSet<TDocument> SetName(string indexName)
        {
            SetMap.SetName(indexName: indexName);
            return this;
        }

        /// <summary>
        ///     动态设置索引名称、别名
        /// </summary>
        /// <param name="indexName"> 新的索引名称 </param>
        /// <param name="aliasNames"> 别名 </param>
        public IndexSet<TDocument> SetName(string indexName, params string[] aliasNames)
        {
            SetMap.SetName(indexName: indexName, aliasNames: aliasNames);
            return this;
        }

        /// <summary>
        ///     条件
        /// </summary>
        public IndexSet<TDocument> Where(Func<QueryContainerDescriptor<TDocument>, QueryContainer> query)
        {
            _query = _query && query.Invoke(new QueryContainerDescriptor<TDocument>());
            return this;
        }

        /// <summary>
        ///     条件
        /// </summary>
        public IndexSet<TDocument> Where(Expression<Func<TDocument, bool>> query)
        {
            _query = _query && new WhereVisitor<TDocument>().Visit(query);
            return this;
        }

        /// <summary>
        ///     排序
        /// </summary>
        public IndexSet<TDocument> Asc<TKey>(Expression<Func<TDocument, TKey>> asc)
        {
            _sort = s => s.Ascending(asc);
            return this;
        }

        /// <summary>
        ///     排序
        /// </summary>
        public IndexSet<TDocument> Desc<TKey>(Expression<Func<TDocument, TKey>> desc)
        {
            _sort = s => s.Descending(desc);
            return this;
        }

        /// <summary>
        ///     筛选字段
        /// </summary>
        /// <param name="fields"> </param>
        /// <returns> </returns>
        public IndexSet<TDocument> Select(params Expression<Func<TDocument, object>>[] fields)
        {
            _selectFields = fields;
            return this;
        }

        /// <summary>
        ///     设置所属上下文
        /// </summary>
        /// <param name="context"> 上下文 </param>
        /// <param name="pInfo"> 当前在上下文中的属性 </param>
        internal void SetContext(EsContext context, PropertyInfo pInfo)
        {
            _esContext = context;
            _pInfo     = pInfo;
        }

        /// <summary>
        ///     写入数据
        /// </summary>
        [TrackElasticsearch]
        public virtual bool Insert(TDocument model)
        {
            WhenNotExistsAddIndex();
            var result = Client.Index(request: new IndexRequest<TDocument>(documentWithId: model, index: SetMap.IndexName));
            if (!result.IsValid)
            {
                var error = result.OriginalException != null ? result.OriginalException.Message : result.DebugInformation;
                IocManager.Instance.Logger<IndexSet<TDocument>>().LogError(message: $"索引失败：{typeof(TDocument).FullName} \r\n" + error);
            }
            return result.IsValid;
        }

        /// <summary>
        ///     写入数据
        /// </summary>
        [TrackElasticsearch]
        public virtual async Task<bool> InsertAsync(TDocument model)
        {
            WhenNotExistsAddIndex();
            var result = await Client.IndexAsync(request: new IndexRequest<TDocument>(documentWithId: model, index: SetMap.IndexName));
            if (!result.IsValid)
            {
                var error = result.OriginalException != null ? result.OriginalException.Message : result.DebugInformation;
                IocManager.Instance.Logger<IndexSet<TDocument>>().LogError(message: $"索引失败：{typeof(TDocument).FullName} \r\n" + error);
            }

            return result.IsValid;
        }

        /// <summary>
        ///     批量写入数据
        /// </summary>
        [TrackElasticsearch]
        public virtual bool Insert(IEnumerable<TDocument> lst)
        {
            if (!lst.Any()) return false;
            WhenNotExistsAddIndex();
            var result = Client.IndexMany(objects: lst, index: SetMap.IndexName);
            if (!result.IsValid)
            {
                var error = result.OriginalException != null ? result.OriginalException.Message : result.DebugInformation;
                IocManager.Instance.Logger<IndexSet<TDocument>>().LogError(message: $"索引失败：{typeof(TDocument).FullName}，数据量：{lst.Count()}条 \r\n" + error);
            }

            return result.IsValid;
        }

        /// <summary>
        ///     批量写入数据
        /// </summary>
        [TrackElasticsearch]
        public virtual async Task<bool> InsertAsync(IEnumerable<TDocument> lst)
        {
            if (!lst.Any()) return false;
            WhenNotExistsAddIndex();
            var result = await Client.IndexManyAsync(objects: lst, index: SetMap.IndexName);
            if (!result.IsValid)
            {
                var error = result.OriginalException != null ? result.OriginalException.Message : result.DebugInformation;
                IocManager.Instance.Logger<IndexSet<TDocument>>().LogError(message: $"索引失败：{typeof(TDocument).FullName}，数据量：{lst.Count()}条 \r\n" + error);
            }

            return result.IsValid;
        }

        /// <summary>
        ///     获取全部数据列表（支持获取全部数据）
        /// </summary>
        [TrackElasticsearch]
        public PooledList<TDocument> ToScrollList()
        {
            var size       = 1000;
            var scrollTime = new Time(timeSpan: TimeSpan.FromSeconds(value: 30));
            var searchResponse = Client.Search<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(SetMap.AliasNames.ToArray()).Size(size: size).Scroll(scroll: scrollTime);
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new PooledList<TDocument>();
                throw searchResponse.OriginalException;
            }


            // 查询超过1万条记录时，使用滚动（类似游标）方式实现
            PooledList<TDocument> Scroll()
            {
                var lst = new PooledList<TDocument>();
                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new PooledList<TDocument>();
                    throw searchResponse.OriginalException;
                }

                lst.AddRange(collection: searchResponse.Documents.ToPooledList());

                // 数量相等，说明还没有读完全部数据
                while (searchResponse.Documents.Count == size)
                {
                    searchResponse = Client.Scroll<TDocument>(scroll: scrollTime, scrollId: searchResponse.ScrollId);
                    if (searchResponse.Documents.Count > 0) lst.AddRange(collection: searchResponse.Documents.ToPooledList());
                }

                Client.ClearScroll(selector: s => s.ScrollId(searchResponse.ScrollId));
                return lst;
            }

            return Scroll();
        }

        /// <summary>
        ///     获取全部数据列表（支持获取全部数据）
        /// </summary>
        [TrackElasticsearch]
        public async Task<PooledList<TDocument>> ToScrollListAsync()
        {
            var size       = 1000;
            var scrollTime = new Time(timeSpan: TimeSpan.FromSeconds(value: 30));
            var searchResponse = await Client.SearchAsync<TDocument>(selector: s =>
            {
                var searchDescriptor = s.Index(index: SetMap.AliasNames.ToArray()).Size(size: size).Scroll(scroll: scrollTime);
                //if (_query.Count  > 0) searchDescriptor     = searchDescriptor.Query(query: q => q.Bool(selector: b => b.Must(queries: _query)));
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            async Task<PooledList<TDocument>> ScrollAsync()
            {
                var lst = new PooledList<TDocument>();
                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new PooledList<TDocument>();
                    throw searchResponse.OriginalException;
                }

                lst.AddRange(collection: searchResponse.Documents.ToPooledList());

                // 数量相等，说明还没有读完全部数据
                while (searchResponse.Documents.Count == size)
                {
                    searchResponse = await Client.ScrollAsync<TDocument>(scroll: scrollTime, scrollId: searchResponse.ScrollId);
                    if (searchResponse.Documents.Count > 0) lst.AddRange(collection: searchResponse.Documents.ToPooledList());
                }

                await Client.ClearScrollAsync(selector: s => s.ScrollId(searchResponse.ScrollId));
                return lst;
            }

            return await ScrollAsync();
        }

        /// <summary>
        ///     获取全部数据列表（支持取10000条以内）
        /// </summary>
        public PooledList<TDocument> ToList() => ToList(top: 10000);

        /// <summary>
        ///     获取全部数据列表（支持取10000条以内）
        /// </summary>
        public Task<PooledList<TDocument>> ToListAsync() => ToListAsync(top: 10000);

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top"> 显示前多少条数据 </param>
        [TrackElasticsearch]
        public PooledList<TDocument> ToList(int top)
        {
            var searchResponse = Client.Search<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(index: SetMap.AliasNames.ToArray());
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (top           > 0) searchDescriptor     = searchDescriptor.Size(size: top);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new PooledList<TDocument>();
                throw searchResponse.OriginalException;
            }

            return searchResponse.Documents.ToPooledList();
        }

        /// <summary>
        ///     获取数据列表
        /// </summary>
        /// <param name="top"> 显示前多少条数据 </param>
        [TrackElasticsearch]
        public async Task<PooledList<TDocument>> ToListAsync(int top)
        {
            var searchResponse = await Client.SearchAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(index: SetMap.AliasNames.ToArray());
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (top           > 0) searchDescriptor     = searchDescriptor.Size(size: top);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new PooledList<TDocument>();
                throw searchResponse.OriginalException;
            }

            return searchResponse.Documents.ToPooledList();
        }

        /// <summary>
        ///     获取数据列表（不推荐翻页超过1万条数据）
        /// </summary>
        /// <param name="pageSize"> 显示每页多少条数据 </param>
        /// <param name="pageIndex"> 索引页 </param>
        [TrackElasticsearch]
        public PageList<TDocument> ToPageList(int pageSize, int pageIndex)
        {
            var from                = 0;
            if (pageIndex > 1) from = (pageIndex - 1) * pageSize;

            var searchResponse = Client.Search<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(index: SetMap.AliasNames).Size(size: pageSize).From(from: from);
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ApiCall.HttpStatusCode == 404) return new PageList<TDocument>();
                throw searchResponse.OriginalException;
            }

            return new PageList<TDocument>(searchResponse.Documents.ToPooledList(), searchResponse.Total);
        }

        /// <summary>
        ///     获取数据列表（不推荐翻页超过1万条数据）
        /// </summary>
        /// <param name="pageSize"> 显示每页多少条数据 </param>
        /// <param name="pageIndex"> 索引页 </param>
        [TrackElasticsearch]
        public async Task<PageList<TDocument>> ToPageListAsync(int pageSize, int pageIndex)
        {
            var from                = 0;
            if (pageIndex > 1) from = (pageIndex - 1) * pageSize;

            var searchResponse = await Client.SearchAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(index: SetMap.AliasNames).Size(size: pageSize).From(from: from);
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ApiCall.HttpStatusCode == 404) return new PageList<TDocument>();
                throw searchResponse.OriginalException;
            }

            return new PageList<TDocument>(searchResponse.Documents.ToPooledList(), searchResponse.Total);
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        [TrackElasticsearch]
        public TValue GetValue<TValue>(Expression<Func<TDocument, object>> select)
        {
            var searchResponse = Client.Search<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames).Size(size: 1).Source(selector: s => s.Includes(fields: i => i.Fields(select)));
                if (_query != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort  != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ApiCall.HttpStatusCode == 404) return default;
                throw searchResponse.OriginalException;
            }

            var entity = searchResponse.Hits?.FirstOrDefault()?.Source;
            return entity == null ? default : select.Compile().Invoke(arg: entity).ConvertType(defValue: default(TValue));
        }

        /// <summary>
        ///     获取单个值
        /// </summary>
        [TrackElasticsearch]
        public async Task<TValue> GetValueAsync<TValue>(Expression<Func<TDocument, object>> select)
        {
            var searchResponse = await Client.SearchAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames).Size(size: 1).Source(selector: s => s.Includes(fields: i => i.Fields(select)));
                if (_query != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort  != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError?.Error.Type == "index_not_found_exception") return default;
                throw searchResponse.OriginalException;
            }

            var entity = searchResponse.Hits?.FirstOrDefault()?.Source;
            return entity == null ? default : select.Compile().Invoke(arg: entity).ConvertType(defValue: default(TValue));
        }

        /// <summary>
        ///     获取单个实体
        /// </summary>
        [TrackElasticsearch]
        public TDocument ToEntity()
        {
            var searchResponse = Client.Search<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(index: SetMap.AliasNames).Size(size: 1);
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return null;
                throw searchResponse.OriginalException;
            }

            return searchResponse.Hits?.FirstOrDefault()?.Source;
        }

        /// <summary>
        ///     获取单个实体
        /// </summary>
        [TrackElasticsearch]
        public async Task<TDocument> ToEntityAsync()
        {
            var searchResponse = await Client.SearchAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                        = s.Index(index: SetMap.AliasNames).Size(size: 1);
                if (_query        != null) searchDescriptor = searchDescriptor.Query(query: q => _query);
                if (_sort         != null) searchDescriptor = searchDescriptor.Sort(selector: _sort);
                if (_selectFields != null) searchDescriptor = searchDescriptor.Source(selector: s => s.Includes(fields: i => i.Fields(fields: _selectFields)));
                return searchDescriptor;
            });

            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return null;
                throw searchResponse.OriginalException;
            }

            return searchResponse.Hits?.FirstOrDefault()?.Source;
        }

        /// <summary>
        ///     记录是否存在
        /// </summary>
        [TrackElasticsearch]
        public bool IsExists()
        {
            var searchResponse = Client.Count<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return searchDescriptor;
            });

            // 异常则抛出
            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return false;
                throw new Exception(message: searchResponse.ServerError.Error.ToString());
            }

            return searchResponse.Count > 0;
        }

        /// <summary>
        ///     记录是否存在
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> IsExistsAsync()
        {
            var searchResponse = await Client.CountAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return searchDescriptor;
            });

            // 异常则抛出
            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return false;
                throw new Exception(message: searchResponse.ServerError.Error.ToString());
            }

            return searchResponse.Count > 0;
        }

        /// <summary>
        ///     查询数量
        /// </summary>
        [TrackElasticsearch]
        public long Count()
        {
            var searchResponse = Client.Count<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return searchDescriptor;
            });

            // 异常则抛出
            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return 0;
                throw new Exception(message: searchResponse.ServerError.Error.ToString());
            }

            return searchResponse.Count;
        }

        /// <summary>
        ///     查询数量
        /// </summary>
        [TrackElasticsearch]
        public async Task<long> CountAsync()
        {
            var searchResponse = await Client.CountAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return searchDescriptor;
            });

            // 异常则抛出
            if (!searchResponse.IsValid)
            {
                if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return 0;
                throw new Exception(message: searchResponse.ServerError.Error.ToString());
            }

            return searchResponse.Count;
        }

        /// <summary>
        ///     修改（指定ID的方式）
        /// </summary>
        [TrackElasticsearch]
        public bool Update(string id, TDocument entity)
        {
            var result = Client.Update<TDocument>(id: id, selector: s => s.Index(index: SetMap.IndexName).Doc(@object: entity));
            return result.IsValid;
        }

        /// <summary>
        ///     修改（指定ID的方式）
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> UpdateAsync(string id, TDocument entity)
        {
            var result = await Client.UpdateAsync<TDocument>(id: id, selector: s => s.Index(index: SetMap.IndexName).Doc(@object: entity));
            return result.IsValid;
        }

        /// <summary>
        ///     修改
        /// </summary>
        [TrackElasticsearch]
        public bool Update(object entity, bool firstCharToLower = true)
        {
            var result = Client.UpdateByQuery<TDocument>(selector: o =>
            {
                var searchDescriptor                 = o.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return Script(desc: searchDescriptor, entity: entity, firstCharToLower: firstCharToLower);
            });

            return result.IsValid;
        }

        /// <summary>
        ///     修改
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> UpdateAsync(object entity, bool firstCharToLower = true)
        {
            var result = await Client.UpdateByQueryAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return Script(desc: searchDescriptor, entity: entity, firstCharToLower: firstCharToLower);
            });

            return result.IsValid;
        }

        /// <summary>
        ///     生成更新脚本
        /// </summary>
        /// <param name="desc"> ES对象 </param>
        /// <param name="entity"> 要更新的对象（注意，这里的对象属性必须跟PO一致） </param>
        /// <param name="firstCharToLower"> 是否首字母小写（默认小写） </param>
        private UpdateByQueryDescriptor<TDocument> Script(UpdateByQueryDescriptor<TDocument> desc, object entity, bool firstCharToLower)
        {
            using var lstScript = new PooledList<string>();
            var       dicValue  = new Dictionary<string, object>();

            foreach (var property in entity.GetType().GetProperties())
            {
                var value = PropertyGetCacheManger.Cache(property, entity);
                // 值为null时，不更新
                if (value == null) continue;

                var field = GetFieldName(property: property);
                // 首字母小写
                if (firstCharToLower)
                {
                    var firstChar = field.Substring(startIndex: 0, length: 1).ToLower();
                    if (field.Length > 1)
                        field = firstChar + field.Substring(startIndex: 1);
                    else
                        field = firstChar;
                }

                lstScript.Add(item: $"ctx._source.{field}=params.{field}");
                dicValue.Add(key: field, value: value);
            }

            return desc.Script(scriptSelector: s => s.Source(script: string.Join(separator: ";", values: lstScript)).Params(dicValue));
        }

        /// <summary>
        ///     删除数据（指定Id的方式）
        /// </summary>
        [TrackElasticsearch]
        public bool Delete(string id)
        {
            var searchResponse = Client.Delete<TDocument>(id: id);
            return searchResponse.IsValid;
        }

        /// <summary>
        ///     删除数据（指定Id的方式）
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> DeleteAsync(string id)
        {
            var searchResponse = await Client.DeleteAsync<TDocument>(id: id);
            return searchResponse.IsValid;
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        [TrackElasticsearch]
        public bool Delete()
        {
            var searchResponse = Client.DeleteByQuery<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return searchDescriptor;
            });
            return searchResponse.IsValid;
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> DeleteAsync()
        {
            var searchResponse = await Client.DeleteByQueryAsync<TDocument>(selector: s =>
            {
                var searchDescriptor                 = s.Index(index: SetMap.AliasNames);
                if (_query != null) searchDescriptor = searchDescriptor.Query(q => _query);
                return searchDescriptor;
            });
            return searchResponse.IsValid;
        }

        /// <summary>
        ///     获取ES实体字段名
        /// </summary>
        private string GetFieldName(PropertyInfo property)
        {
            var number = property.GetAttribute<NumberAttribute>();
            if (number != null)
                if (!string.IsNullOrWhiteSpace(value: number.Name))
                    return number.Name;

            var keyword = property.GetAttribute<KeywordAttribute>();
            if (keyword != null)
                if (!string.IsNullOrWhiteSpace(value: keyword.Name))
                    return keyword.Name;

            var date = property.GetAttribute<DateAttribute>();
            if (date != null)
                if (!string.IsNullOrWhiteSpace(value: date.Name))
                    return date.Name;

            var text = property.GetAttribute<TextAttribute>();
            if (text != null)
                if (!string.IsNullOrWhiteSpace(value: text.Name))
                    return text.Name;

            var obj = property.GetAttribute<ObjectAttribute>();
            if (obj != null)
                if (!string.IsNullOrWhiteSpace(value: obj.Name))
                    return obj.Name;

            return property.Name;
        }

        /// <summary>
        ///     索引不存在时，创建索引
        /// </summary>
        protected void WhenNotExistsAddIndex()
        {
            if (!IndexCache.ContainsKey(key: SetMap.IndexName) || !IndexCache[key: SetMap.IndexName])
            {
                lock (objLock)
                {
                    if (!Client.Indices.Exists(index: SetMap.IndexName).Exists) IndexCache[key: SetMap.IndexName] = CreateIndex();
                }
            }
        }

        /// <summary>
        ///     创建索引
        /// </summary>
        [TrackElasticsearch]
        protected bool CreateIndex()
        {
            var rsp = Client.Indices.Create(index: SetMap.IndexName, selector: c => c
                                                                                    .Map<TDocument>(selector: m => m.AutoMap())
                                                                                    .Aliases(selector: des =>
                                                                                    {
                                                                                        foreach (var aliasName in SetMap.AliasNames) des.Alias(alias: aliasName);
                                                                                        return des;
                                                                                    })
                                                                                    .Settings(selector: s => s.NumberOfReplicas(numberOfReplicas: SetMap.ReplicasCount).NumberOfShards(numberOfShards: SetMap.ShardsCount).RefreshInterval(SetMap.RefreshInterval))
                                           );
            if (!rsp.IsValid) throw new Exception(message: $"索引创建失败：Uri={string.Join(",", Client.ConnectionSettings.ConnectionPool.Nodes.Select(o => o.Uri.ToString()))}，Index={SetMap.IndexName}，AliasNames={string.Join(",", SetMap.AliasNames)}{rsp.OriginalException.Message}");
            return true;
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        [TrackElasticsearch]
        public bool DropIndex()
        {
            var result = Client.Indices.Delete(SetMap.IndexName);

            // 移除本地索引缓存
            if (result.IsValid) IndexCache[key: SetMap.IndexName] = false;
            return result.IsValid;
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> DropIndexAsync()
        {
            var result = await Client.Indices.DeleteAsync(SetMap.IndexName);

            // 移除本地索引缓存
            if (result.IsValid) IndexCache[key: SetMap.IndexName] = false;
            return result.IsValid;
        }

        /// <summary>
        /// 刷新索引
        /// </summary>
        [TrackElasticsearch]
        public bool RefreshIndex() => Client.Indices.Refresh(SetMap.IndexName).IsValid;

        /// <summary>
        /// 刷新索引
        /// </summary>
        [TrackElasticsearch]
        public async Task<bool> RefreshIndexAsync()
        {
            var result = await Client.Indices.RefreshAsync(SetMap.IndexName);
            return result.IsValid;
        }
    }
}