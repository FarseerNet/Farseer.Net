using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FS.Core.LinkTrack;
using FS.DI;
using FS.ElasticSearch.Internal;
using FS.ElasticSearch.Map;
using FS.Extends;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace FS.ElasticSearch
{
    /// <summary>
    /// 索引操作
    /// </summary>
    public class IndexSet<TDocument> where TDocument : class, new()
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
        /// 条件语句
        /// </summary>
        private List<Func<QueryContainerDescriptor<TDocument>, QueryContainer>> _query = new();

        /// <summary>
        /// 排序语句
        /// </summary>
        private Func<SortDescriptor<TDocument>, IPromise<IList<ISort>>> _sort;

        /// <summary>
        /// 筛选字段
        /// </summary>
        private Expression<Func<TDocument, object>>[] _selectFields;

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
        }

        /// <summary>
        /// 动态设置索引名称、别名
        /// </summary>
        /// <param name="indexName">新的索引名称</param>
        /// <param name="shardsCount">新的分片数量</param>
        /// <param name="replicasCount">新的副本数量</param>
        /// <param name="aliasNames">别名</param>
        public IndexSet<TDocument> SetName(string indexName, int shardsCount = 3, int replicasCount = 1, params string[] aliasNames)
        {
            SetMap.SetName(indexName, shardsCount, replicasCount, aliasNames);
            return this;
        }

        /// <summary>
        /// 条件
        /// </summary>
        public IndexSet<TDocument> Where(Func<QueryContainerDescriptor<TDocument>, QueryContainer> query)
        {
            _query.Add(query);
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public IndexSet<TDocument> Sort(Func<SortDescriptor<TDocument>, IPromise<IList<ISort>>> sort)
        {
            _sort = sort;
            return this;
        }

        /// <summary>
        /// 筛选字段
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public IndexSet<TDocument> Select(params Expression<Func<TDocument, object>>[] fields)
        {
            _selectFields = fields;
            return this;
        }

        /// <summary>
        /// ES客户端
        /// </summary>
        public IElasticClient Client { get; }

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

        /// <summary>
        /// 写入数据
        /// </summary>
        public virtual bool Insert(TDocument model)
        {
            using (FsLinkTrack.TrackElasticsearch("Insert"))
            {
                WhenNotExistsAddIndex();
                var result = Client.Index(new IndexRequest<TDocument>(model, SetMap.IndexName));
                if (!result.IsValid)
                {
                    IocManager.Instance.Logger<IndexSet<TDocument>>().LogError($"索引失败：{JsonConvert.SerializeObject(model)} \r\n" + result.OriginalException.Message);
                }

                return result.IsValid;
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public virtual async Task<bool> InsertAsync(TDocument model)
        {
            using (FsLinkTrack.TrackElasticsearch("InsertAsync"))
            {
                WhenNotExistsAddIndex();
                var result = await Client.IndexAsync(new IndexRequest<TDocument>(model, SetMap.IndexName));
                if (!result.IsValid)
                {
                    IocManager.Instance.Logger<IndexSet<TDocument>>().LogError($"索引失败：{JsonConvert.SerializeObject(model)} \r\n" + result.OriginalException.Message);
                }

                return result.IsValid;
            }
        }

        /// <summary>
        /// 批量写入数据
        /// </summary>
        public virtual bool Insert(List<TDocument> lst)
        {
            using (FsLinkTrack.TrackElasticsearch("Insert"))
            {
                WhenNotExistsAddIndex();
                var result = Client.IndexMany(lst, SetMap.IndexName);
                if (!result.IsValid)
                {
                    IocManager.Instance.Logger<IndexSet<TDocument>>().LogError($"索引失败：{JsonConvert.SerializeObject(lst)} \r\n" + result.OriginalException.Message);
                }

                return result.IsValid;
            }
        }

        /// <summary>
        /// 批量写入数据
        /// </summary>
        public virtual async Task<bool> InsertAsync(List<TDocument> lst)
        {
            using (FsLinkTrack.TrackElasticsearch("InsertAsync"))
            {
                WhenNotExistsAddIndex();
                var result = await Client.IndexManyAsync(lst, SetMap.IndexName);
                if (!result.IsValid)
                {
                    IocManager.Instance.Logger<IndexSet<TDocument>>().LogError($"索引失败：{JsonConvert.SerializeObject(lst)} \r\n" + result.OriginalException.Message);
                }

                return result.IsValid;
            }
        }

        /// <summary>
        /// 获取全部数据列表（支持获取全部数据）
        /// </summary>
        public List<TDocument> ToScrollList()
        {
            using (FsLinkTrack.TrackElasticsearch("ToScrollList"))
            {
                var size       = 1000;
                var scrollTime = new Time(TimeSpan.FromSeconds(30));
                var searchResponse = Client.Search<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames).Size(size).Scroll(scrollTime);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new List<TDocument>();
                    throw searchResponse.OriginalException;
                }


                // 查询超过1万条记录时，使用滚动（类似游标）方式实现
                List<TDocument> Scroll()
                {
                    var lst = new List<TDocument>();
                    if (!searchResponse.IsValid)
                    {
                        if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new List<TDocument>();
                        throw searchResponse.OriginalException;
                    }

                    lst.AddRange(searchResponse.Documents.ToList());

                    // 数量相等，说明还没有读完全部数据
                    while (searchResponse.Documents.Count == size)
                    {
                        searchResponse = Client.Scroll<TDocument>(scrollTime, searchResponse.ScrollId);
                        if (searchResponse.Documents.Count > 0)
                        {
                            lst.AddRange(searchResponse.Documents.ToList());
                        }
                    }

                    Client.ClearScroll(s => s.ScrollId(searchResponse.ScrollId));
                    return lst;
                }

                return Scroll();
            }
        }

        /// <summary>
        /// 获取全部数据列表（支持获取全部数据）
        /// </summary>
        public async Task<List<TDocument>> ToScrollListAsync()
        {
            using (FsLinkTrack.TrackElasticsearch("ToScrollListAsync"))
            {
                var size       = 1000;
                var scrollTime = new Time(TimeSpan.FromSeconds(30));
                var searchResponse = await Client.SearchAsync<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames).Size(size).Scroll(scrollTime);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                async Task<List<TDocument>> ScrollAsync()
                {
                    var lst = new List<TDocument>();
                    if (!searchResponse.IsValid)
                    {
                        if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new List<TDocument>();
                        throw searchResponse.OriginalException;
                    }

                    lst.AddRange(searchResponse.Documents.ToList());

                    // 数量相等，说明还没有读完全部数据
                    while (searchResponse.Documents.Count == size)
                    {
                        searchResponse = await Client.ScrollAsync<TDocument>(scrollTime, searchResponse.ScrollId);
                        if (searchResponse.Documents.Count > 0)
                        {
                            lst.AddRange(searchResponse.Documents.ToList());
                        }
                    }

                    await Client.ClearScrollAsync(s => s.ScrollId(searchResponse.ScrollId));
                    return lst;
                }

                return await ScrollAsync();
            }
        }

        /// <summary>
        /// 获取全部数据列表（支持取10000条以内）
        /// </summary>
        public List<TDocument> ToList() => ToList(10000);

        /// <summary>
        /// 获取全部数据列表（支持取10000条以内）
        /// </summary>
        public Task<List<TDocument>> ToListAsync() => ToListAsync(10000);

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="top">显示前多少条数据</param>
        public List<TDocument> ToList(int top)
        {
            using (FsLinkTrack.TrackElasticsearch("ToList"))
            {
                var searchResponse = Client.Search<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (top > 0) searchDescriptor               = searchDescriptor.Size(top);
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new List<TDocument>();
                    throw searchResponse.OriginalException;
                }

                return searchResponse.Documents.ToList();
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="top">显示前多少条数据</param>
        public async Task<List<TDocument>> ToListAsync(int top)
        {
            using (FsLinkTrack.TrackElasticsearch("ToListAsync"))
            {
                var searchResponse = await Client.SearchAsync<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (top > 0) searchDescriptor               = searchDescriptor.Size(top);
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new List<TDocument>();
                    throw searchResponse.OriginalException;
                }

                return searchResponse.Documents.ToList();
            }
        }

        /// <summary>
        /// 获取数据列表（不推荐翻页超过1万条数据）
        /// </summary>
        /// <param name="pageSize">显示每页多少条数据</param>
        /// <param name="pageIndex">索引页 </param>
        /// <param name="recordCount">命中的总记录数 </param>
        public List<TDocument> ToList(int pageSize, int pageIndex, out long recordCount)
        {
            using (FsLinkTrack.TrackElasticsearch("ToList"))
            {
                recordCount = 0;
                var from                = 0;
                if (pageIndex > 1) from = (pageIndex - 1) * pageSize;

                var searchResponse = Client.Search<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames).Size(pageSize).From(from);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return new List<TDocument>();
                    throw searchResponse.OriginalException;
                }

                recordCount = searchResponse.Total;
                return searchResponse.Documents.ToList();
            }
        }

        /// <summary>
        /// 获取单个值
        /// </summary>
        public TValue GetValue<TValue>(Expression<Func<TDocument, object>> select)
        {
            using (FsLinkTrack.TrackElasticsearch("GetValue"))
            {
                var searchResponse = Client.Search<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames).Size(1).Source(s => s.Includes(i => i.Fields(select)));
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor    = searchDescriptor.Sort(_sort);
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return default;
                    throw searchResponse.OriginalException;
                }

                var entity = searchResponse.Hits?.FirstOrDefault()?.Source;
                return entity == null ? default : select.Compile().Invoke(entity).ConvertType(default(TValue));
            }
        }

        /// <summary>
        /// 获取单个值
        /// </summary>
        public async Task<TValue> GetValueAsync<TValue>(Expression<Func<TDocument, object>> select)
        {
            using (FsLinkTrack.TrackElasticsearch("GetValueAsync"))
            {
                var searchResponse = await Client.SearchAsync<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames).Size(1).Source(s => s.Includes(i => i.Fields(select)));
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor    = searchDescriptor.Sort(_sort);
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return default;
                    throw searchResponse.OriginalException;
                }

                var entity = searchResponse.Hits?.FirstOrDefault()?.Source;
                return entity == null ? default : select.Compile().Invoke(entity).ConvertType(default(TValue));
            }
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        public TDocument ToEntity()
        {
            using (FsLinkTrack.TrackElasticsearch("ToEntity"))
            {
                var searchResponse = Client.Search<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames).Size(1);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return null;
                    throw searchResponse.OriginalException;
                }

                return searchResponse.Hits?.FirstOrDefault()?.Source;
            }
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        public async Task<TDocument> ToEntityAsync()
        {
            using (FsLinkTrack.TrackElasticsearch("ToEntityAsync"))
            {
                var searchResponse = await Client.SearchAsync<TDocument>(s =>
                {
                    var searchDescriptor                        = s.Index(SetMap.AliasNames).Size(1);
                    if (_query.Count > 0) searchDescriptor      = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    if (_sort != null) searchDescriptor         = searchDescriptor.Sort(_sort);
                    if (_selectFields != null) searchDescriptor = searchDescriptor.Source(s => s.Includes(i => i.Fields(_selectFields)));
                    return searchDescriptor;
                });

                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return null;
                    throw searchResponse.OriginalException;
                }

                return searchResponse.Hits?.FirstOrDefault()?.Source;
            }
        }

        /// <summary>
        /// 记录是否存在
        /// </summary>
        public bool IsExists()
        {
            using (FsLinkTrack.TrackElasticsearch("IsExists"))
            {
                var searchResponse = Client.Count<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return searchDescriptor;
                });

                // 异常则抛出
                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return false;
                    throw new Exception(searchResponse.ServerError.Error.ToString());
                }

                return searchResponse.Count > 0;
            }
        }

        /// <summary>
        /// 记录是否存在
        /// </summary>
        public async Task<bool> IsExistsAsync()
        {
            using (FsLinkTrack.TrackElasticsearch("IsExistsAsync"))
            {
                var searchResponse = await Client.CountAsync<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return searchDescriptor;
                });

                // 异常则抛出
                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return false;
                    throw new Exception(searchResponse.ServerError.Error.ToString());
                }

                return searchResponse.Count > 0;
            }
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        public long Count()
        {
            using (FsLinkTrack.TrackElasticsearch("Count"))
            {
                var searchResponse = Client.Count<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return searchDescriptor;
                });

                // 异常则抛出
                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return 0;
                    throw new Exception(searchResponse.ServerError.Error.ToString());
                }

                return searchResponse.Count;
            }
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        public async Task<long> CountAsync()
        {
            using (FsLinkTrack.TrackElasticsearch("CountAsync"))
            {
                var searchResponse = await Client.CountAsync<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return searchDescriptor;
                });

                // 异常则抛出
                if (!searchResponse.IsValid)
                {
                    if (searchResponse.ServerError.Error.Type == "index_not_found_exception") return 0;
                    throw new Exception(searchResponse.ServerError.Error.ToString());
                }

                return searchResponse.Count;
            }
        }

        /// <summary>
        /// 修改（指定ID的方式）
        /// </summary>
        public bool Update(string id, TDocument entity)
        {
            using (FsLinkTrack.TrackElasticsearch("Update"))
            {
                var result = Client.Update<TDocument>(id, s => s.Index(SetMap.IndexName).Doc(entity));
                return result.IsValid;
            }
        }

        /// <summary>
        /// 修改（指定ID的方式）
        /// </summary>
        public async Task<bool> UpdateAsync(string id, TDocument entity)
        {
            using (FsLinkTrack.TrackElasticsearch("Update"))
            {
                var result = await Client.UpdateAsync<TDocument>(id, s => s.Index(SetMap.IndexName).Doc(entity));
                return result.IsValid;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public bool Update(object entity, bool firstCharToLower = true)
        {
            using (FsLinkTrack.TrackElasticsearch("Update"))
            {
                var result = Client.UpdateByQuery<TDocument>(o =>
                {
                    var searchDescriptor                   = o.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return Script(searchDescriptor, entity, firstCharToLower);
                });

                return result.IsValid;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public async Task<bool> UpdateAsync(object entity, bool firstCharToLower = true)
        {
            using (FsLinkTrack.TrackElasticsearch("UpdateAsync"))
            {
                var result = await Client.UpdateByQueryAsync<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return Script(searchDescriptor, entity, firstCharToLower);
                });


                return result.IsValid;
            }
        }

        /// <summary>
        /// 生成更新脚本
        /// </summary>
        /// <param name="desc">ES对象</param>
        /// <param name="entity">要更新的对象（注意，这里的对象属性必须跟PO一致）</param>
        /// <param name="firstCharToLower">是否首字母小写（默认小写）</param>
        private UpdateByQueryDescriptor<TDocument> Script(UpdateByQueryDescriptor<TDocument> desc, object entity, bool firstCharToLower)
        {
            var lstScript = new List<string>();
            var dicValue  = new Dictionary<string, object>();

            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                string field = GetFieldName(property);
                // 首字母小写
                if (firstCharToLower)
                {
                    var firstChar = field.Substring(0, 1).ToLower();
                    if (field.Length > 1) field = firstChar + field.Substring(1);
                    else field                  = firstChar;
                }

                lstScript.Add($"ctx._source.{field}=params.{field}");
                dicValue.Add(field, property.GetValue(entity));
            }

            return desc.Script(s => s.Source(string.Join(";", lstScript)).Params(dicValue));
        }

        /// <summary>
        /// 删除数据（指定Id的方式）
        /// </summary>
        public bool Delete(string id)
        {
            using (FsLinkTrack.TrackElasticsearch("Delete"))
            {
                var searchResponse = Client.Delete<TDocument>(id);
                return searchResponse.IsValid;
            }
        }

        /// <summary>
        /// 删除数据（指定Id的方式）
        /// </summary>
        public async Task<bool> DeleteAsync(string id)
        {
            using (FsLinkTrack.TrackElasticsearch("DeleteAsync"))
            {
                var searchResponse = await Client.DeleteAsync<TDocument>(id);
                return searchResponse.IsValid;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public bool Delete()
        {
            using (FsLinkTrack.TrackElasticsearch("Delete"))
            {
                var searchResponse = Client.DeleteByQuery<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return searchDescriptor;
                });
                return searchResponse.IsValid;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task<bool> DeleteAsync()
        {
            using (FsLinkTrack.TrackElasticsearch("DeleteAsync"))
            {
                var searchResponse = await Client.DeleteByQueryAsync<TDocument>(s =>
                {
                    var searchDescriptor                   = s.Index(SetMap.AliasNames);
                    if (_query.Count > 0) searchDescriptor = searchDescriptor.Query(q => q.Bool(b => b.Must(_query)));
                    return searchDescriptor;
                });
                return searchResponse.IsValid;
            }
        }

        /// <summary>
        /// 获取ES实体字段名
        /// </summary>
        private string GetFieldName(PropertyInfo property)
        {
            var number = property.GetAttribute<NumberAttribute>();
            if (number != null)
            {
                if (!string.IsNullOrWhiteSpace(number.Name))
                {
                    return number.Name;
                }
            }

            var keyword = property.GetAttribute<KeywordAttribute>();
            if (keyword != null)
            {
                if (!string.IsNullOrWhiteSpace(keyword.Name))
                {
                    return keyword.Name;
                }
            }

            var date = property.GetAttribute<DateAttribute>();
            if (date != null)
            {
                if (!string.IsNullOrWhiteSpace(date.Name))
                {
                    return date.Name;
                }
            }

            var text = property.GetAttribute<TextAttribute>();
            if (text != null)
            {
                if (!string.IsNullOrWhiteSpace(text.Name))
                {
                    return text.Name;
                }
            }

            var obj = property.GetAttribute<ObjectAttribute>();
            if (obj != null)
            {
                if (!string.IsNullOrWhiteSpace(obj.Name))
                {
                    return obj.Name;
                }
            }

            return property.Name;
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
            using (FsLinkTrack.TrackElasticsearch("CreateIndex"))
            {
                var rsp = Client.Indices.Create(SetMap.IndexName, c => c
                    .Map<TDocument>(m => m.AutoMap())
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
        }
    }
}