using System;
using System.Linq;
using FS.Cache;
using FS.DI;
using FS.Reflection;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace FS.Core.Abstract.Data;

/// <summary>
/// 事务执行
/// </summary>
[PSerializable]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class TransactionNameAttribute : MethodInterceptionAspect
{
    public string DbConfigName { get; set; }
    public TransactionNameAttribute(string dbConfigName)
    {
        DbConfigName = dbConfigName;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var dbContextType = IocManager.GetService<ITypeFinder>().Find<ITransaction>().FirstOrDefault(o => o.FullName == "FS.Data.DbContext");
        if (dbContextType == null) throw new FarseerException($"未找到FS.Data.DbContext实现类，无法开启事务。");
        var dbContext = InstanceCacheManger.Cache(dbContextType, DbConfigName);
        // ReSharper disable once MergeCastWithTypeCheck
        if (dbContext is not ITransaction)
        {
            throw new FarseerException($"类型：{dbContextType.FullName} 必须继承DbContext，才能开启事务");
        }
        // 事务执行
        using (var context = (ITransaction)dbContext)
        {
            args.Proceed();
            context.SaveChanges();
        }
    }
}