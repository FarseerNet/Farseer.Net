using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using FS.Data.Cache;
using FS.Data.Client;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Data.Map;
using FS.DI;

namespace FS.Data
{
    /// <summary>
    ///     数据库上下文（使用实例化方式时，必须末尾，执行Commit()方法，否则会产生事务死锁）
    /// </summary>
    public class DbContext : IDisposable
    {
        /// <summary>
        ///     通过数据库配置，连接数据库
        /// </summary>
        /// <param name="name">数据库配置名称</param>
        /// <param name="isUnitOfWork">是否工作单元模式</param>
        public DbContext(string name, bool isUnitOfWork = false) : this(isUnitOfWork)
        {
            _internalContext.ContextConnection = IocManager.Instance.Resolve<IContextConnection>(name);
        }

        /// <summary>
        ///     通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="db">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        /// <param name="dataVer">数据库版本（针对不同的数据库版本的优化）</param>
        /// <param name="isUnitOfWork">是否工作单元模式</param>
        public DbContext(string connectionString, eumDbType db, int commandTimeout = 30, string dataVer = null, bool isUnitOfWork = false) : this(isUnitOfWork)
        {
            _internalContext.ContextConnection = new ContextConnection(connectionString, db, commandTimeout, dataVer);
        }

        /// <summary>
        /// 不初始化ContextConnection（用于动态改变数据库连接方式）
        /// </summary>
        /// <param name="isUnitOfWork">是否工作单元模式</param>
        protected DbContext(bool isUnitOfWork)
        {
            _internalContext = new InternalContext(this.GetType(), isUnitOfWork);
            // 实例化子类中，所有Set属性
            ContextSetTypeCacheManger.Cache(this.GetType()).Item2(this);
        }

        /// <summary>
        ///     实现动态链接数据库（比如：分库）
        /// </summary>
        protected virtual IContextConnection SplitDatabase() => _internalContext.ContextConnection;

        /// <summary>
        ///     创建来自其它上下文的共享
        /// </summary>
        /// <param name="masterContext">其它上下文（主上下文）</param>
        internal static TPo TransactionInstance<TPo>(DbContext masterContext) where TPo : DbContext, new()
        {
            var newInstance = new TPo();
            newInstance._internalContext.TransactionInstance(typeof(TPo), masterContext.InternalContext);
            return newInstance;
        }

        /// <summary>
        ///     静态实例
        /// </summary>
        internal static TPo Data<TPo>() where TPo : DbContext, new()
        {
            // 2016年1月8日
            // 感谢：QQ462492293 疯狂的蜗牛 同学，发现了BUG
            // 场景：在For迭代操作数据库时，提示：【数据库连接池连接均在使用,并且达到了最大值】的错误。
            // 解决：由于此处进行了_internalContext的二次初始化（在SqlSet进行了初始化）。需保证当前_internalContext未被初始化。

            var newInstance = new TPo();
            // 上下文初始化器
            newInstance._internalContext.IsUnitOfWork = true;
            return newInstance;
        }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        public AbsDbProvider DbProvider => InternalContext.DbProvider;

        /// <summary>
        ///     手动编写SQL
        /// </summary>
        public ManualSql ManualSql => InternalContext.ManualSql;

        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        public IContextConnection ContextConnection => InternalContext.ContextConnection;

        /// <summary>
        ///     保存修改
        /// </summary>
        public void SaveChanges()
        {
            // 执行数据库操作
            //var result = InternalContext.QueueManger.CommitAll();

            // 如果开启了事务，则关闭
            if (InternalContext.Executeor.DataBase.IsTransaction)
            {
                InternalContext.Executeor.DataBase.Commit();
                InternalContext.Executeor.DataBase.CloseTran();
            }
            InternalContext.Executeor.DataBase.Close(true);
            //return result;
        }

        /// <summary>
        ///     回滚事务
        /// </summary>
        public void Rollback()
        {
            // 执行数据库操作
            //InternalContext.QueueManger.ClearAll();

            // 如果开启了事务，则关闭
            if (InternalContext.Executeor.DataBase.IsTransaction)
            {
                InternalContext.Executeor.DataBase.Rollback();
                InternalContext.Executeor.DataBase.CloseTran();
            }
            InternalContext.Executeor.DataBase.Close(true);
        }

        /// <summary>
        ///     取消命令合并（不需要调用SaveChange()方法）
        /// </summary>
        public void CancelMergeCommand() => InternalContext.IsUnitOfWork = true;

        /// <summary>
        ///     不以事务方式执行
        /// </summary>
        public void CancelTransaction() => InternalContext.Executeor.DataBase.CloseTran();

        /// <summary>
        ///     改变事务级别
        /// </summary>
        /// <param name="tranLevel">事务方式</param>
        public void ChangeTransaction(IsolationLevel tranLevel) => InternalContext.Executeor.DataBase.OpenTran(tranLevel);

        /// <summary>
        ///     在创建模型时调用
        /// </summary>
        protected virtual void CreateModelInit(Dictionary<string, SetDataMap> map) { }

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
                    // 分库方案
                    if (_internalContext.ContextConnection == null) { _internalContext.ContextConnection = SplitDatabase(); }
                    _internalContext.Initializer();
                }
                if (!_internalContext.IsInitModelName)
                {
                    // 初始化模型映射
                    CreateModelInit(_internalContext.ContextMap.SetDataList.ToDictionary(o => o.TableName));
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
        public TableSet<TEntity> TableSet<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(TableSet<TEntity>), propertyName);
            return new TableSet<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public TableSet<TEntity> TableSet<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new TableSet<TEntity>(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public TableSetCache<TEntity> TableSetCache<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(TableSetCache<TEntity>), propertyName);
            return new TableSetCache<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public TableSetCache<TEntity> TableSetCache<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new TableSetCache<TEntity>(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public ViewSet<TEntity> ViewSet<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(ViewSet<TEntity>), propertyName);
            return new ViewSet<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public ViewSet<TEntity> ViewSet<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new ViewSet<TEntity>(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public ViewSetCache<TEntity> ViewSetCache<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(ViewSetCache<TEntity>), propertyName);
            return new ViewSetCache<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public ViewSetCache<TEntity> ViewSetCache<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new ViewSetCache<TEntity>(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public ProcSet<TEntity> ProcSet<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(ProcSet<TEntity>), propertyName);
            return new ProcSet<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public ProcSet<TEntity> ProcSet<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new ProcSet<TEntity>(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        /// <typeparam name="TEntity"></typeparam>
        public SqlSet<TEntity> SqlSet<TEntity>(string propertyName = null) where TEntity : class, new()
        {
            var pInfo = GetSetPropertyInfo(typeof(SqlSet<TEntity>), propertyName);
            return new SqlSet<TEntity>(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        /// <typeparam name="TEntity"></typeparam>
        public SqlSet<TEntity> SqlSet<TEntity>(PropertyInfo propertyInfo) where TEntity : class, new()
        {
            return new SqlSet<TEntity>(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        public SqlSet SqlSet(string propertyName = null)
        {
            var pInfo = GetSetPropertyInfo(typeof(SqlSet), propertyName);
            return new SqlSet(this, pInfo);
        }

        /// <summary>
        ///     动态返回Set类型
        /// </summary>
        /// <param name="propertyInfo">上下文中的属性</param>
        public SqlSet SqlSet(PropertyInfo propertyInfo)
        {
            return new SqlSet(this, propertyInfo);
        }

        /// <summary>
        ///     动态返回Set类型的属性数据
        /// </summary>
        /// <param name="setType">Set的类型</param>
        /// <param name="propertyName">当有多个相同类型TEntity时，须使用propertyName来寻找唯一</param>
        private PropertyInfo GetSetPropertyInfo(Type setType, string propertyName = null)
        {
            var lstPropertyInfo = this.GetType().GetProperties();
            var lst = lstPropertyInfo.Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.PropertyType == setType).Where(propertyInfo => propertyName == null || propertyInfo.Name == propertyName);
            if (lst == null) { throw new Exception("未找到当前类型的Set属性：" + setType.GetGenericArguments()[0]); }
            if (lst.Count() > 1) { throw new Exception("找到多个Set属性，请指定propertyName确定唯一。：" + setType.GetGenericArguments()[0]); }
            return lst.FirstOrDefault();
        }

        #endregion

        #region 禁用智能提示

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }

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
                InternalContext.Dispose();
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

        #endregion
    }
}