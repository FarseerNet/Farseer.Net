using System;
using FS.Cache;
using FS.Core.Abstract.Data;
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
    public Type DbContextType { get; set; }
    public TransactionAttribute(Type dbContextType)
    {
        DbContextType = dbContextType;
    }

    public override void OnInvoke(MethodInterceptionArgs args)
    {
        var dbContext = InstanceCacheManger.Cache(DbContextType);
        // ReSharper disable once MergeCastWithTypeCheck
        if (dbContext is not ITransaction)
        {
            throw new FarseerException($"类型：{DbContextType.FullName} 必须继承DbContext，才能开启事务");
        }

        // 事务执行
        using (var context = (ITransaction)dbContext)
        {
            args.Proceed();
            context.SaveChanges();
        }
    }
}