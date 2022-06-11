using System;
using System.Linq;
using System.Threading.Tasks;
using FS.Cache;
using FS.Core.Abstract.Data;
using FS.DI;
using FS.Reflection;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FS.Core.AOP.Data;

/// <summary>
/// 事务执行
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class TransactionAttribute : MethodInterceptionAspect
{
    private Type   _dbContextType;
    private string _dbConfigName;
    private object _dbContext;
    public TransactionAttribute(Type dbContextType)
    {
        _dbContextType = dbContextType;
    }
    public TransactionAttribute(string dbConfigName)
    {
        _dbConfigName = dbConfigName;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        CreateDbContext();
        // 事务执行
        using (var context = (ITransaction)_dbContext)
        {
            args.Proceed();
            context.SaveChanges();
        }
    }
    
    public override async Task OnInvokeAsync(MethodInterceptionArgs args)
    {
        CreateDbContext();
        // 事务执行
        using (var context = (ITransaction)_dbContext)
        {
            await args.ProceedAsync();
            context.SaveChanges();
        }
    }
    
    /// <summary>
    /// 动态创建上下文实例
    /// </summary>
    private void CreateDbContext()
    {

        _dbContextType = _dbContextType switch
        {
            null when string.IsNullOrWhiteSpace(_dbConfigName) => throw new FarseerException($"要开启事务时，需传入配置名或dbContextType"),
            null                                               => IocManager.GetService<ITypeFinder>().Find<ITransaction>().FirstOrDefault(o => o.FullName == "FS.Data.DbContext"),
            _                                                  => _dbContextType
        };

        if (_dbContextType == null) throw new FarseerException($"未找到FS.Data.DbContext实现类，无法开启事务。");

        _dbContext = _dbConfigName != null
        ? InstanceCacheManger.Cache(_dbContextType, _dbConfigName)
        : InstanceCacheManger.Cache(_dbContextType);

        // ReSharper disable once MergeCastWithTypeCheck
        if (_dbContext is not ITransaction)
        {
            throw new FarseerException($"类型：{_dbContextType.FullName} 必须继承DbContext，才能开启事务");
        }
    }
}