using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Farseer.Net.Cache;
using Farseer.Net.Data.Internal;

namespace Farseer.Net.Data.Cache
{
    /// <summary>
    ///     保存派生Context的Set类型
    /// </summary>
    internal class ContextSetTypeCacheManger : AbsCacheManger<Type, SetTypesInitializersPair>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private ContextSetTypeCacheManger(Type key) : base(key)
        {
        }

        protected override SetTypesInitializersPair SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                var dbContextParam = Expression.Parameter(typeof (DbContext), "_context");

                var initDelegates = new List<Action<DbContext>>();

                // 一个DbContext对象下的所有Set实体类型
                var setTypeList = new Dictionary<Type, List<string>>();

                // 取得所有Set属性
                foreach (var entityMap in ContextMapCacheManger.Cache(Key).EntityMapList)
                {
                    // 实体类型，Set的泛型类型
                    var entityType = entityMap.Value != null ? entityMap.Key.PropertyType.GetGenericArguments()[0] : null;

                    List<string> propertyName;
                    // 查找这个Set类在当前DbContext中，出现过几次，并记录属性。
                    if (!setTypeList.TryGetValue(entityMap.Key.PropertyType, out propertyName))
                    {
                        propertyName = new List<string>();
                        setTypeList[entityMap.Key.PropertyType] = propertyName;
                    }
                    propertyName.Add(entityMap.Key.Name);

                    // 属性set方法
                    var setter = entityMap.Key.GetSetMethod(nonPublic: true);
                    if (setter != null && setter.IsPublic)
                    {
                        // 上下文的Set方法（不同Set，提供不同的方法）
                        MethodInfo setMethod = null;
                        switch (entityMap.Key.PropertyType.Name)
                        {
                            case "TableSet`1":
                                setMethod = SetInitializer.TableSetMethod.MakeGenericMethod(entityType);
                                break;
                            case "TableSetCache`1":
                                setMethod = SetInitializer.TableSetCacheMethod.MakeGenericMethod(entityType);
                                break;
                            case "ViewSet`1":
                                setMethod = SetInitializer.ViewSetMethod.MakeGenericMethod(entityType);
                                break;
                            case "ViewSetCache`1":
                                setMethod = SetInitializer.ViewSetCacheMethod.MakeGenericMethod(entityType);
                                break;
                            case "ProcSet`1":
                                setMethod = SetInitializer.ProcSetMethod.MakeGenericMethod(entityType);
                                break;
                            case "SqlSet`1":
                                setMethod = SetInitializer.SqlSetMethod.MakeGenericMethod(entityType);
                                break;
                            case "SqlSet":
                                setMethod = SetInitializer.SqlSetNonGenericMethod;
                                break;
                        }

                        // 取得实例化
                        //var dbContextInitializer = typeof(DbContext).GetField("_dbContextInitializer", BindingFlags.NonPublic | BindingFlags.Instance);
                        //var newExpression = Expression.Call(Expression.MakeMemberAccess(dbContextParam, dbContextInitializer), setMethod, Expression.Constant(entityMap.Key));
                        var newExpression = Expression.Call(dbContextParam, setMethod, Expression.Constant(entityMap.Key));

                        // 赋值
                        var setExpression = Expression.Call(Expression.Convert(dbContextParam, Key), setter, newExpression);
                        initDelegates.Add(Expression.Lambda<Action<DbContext>>(setExpression, dbContextParam).Compile());
                    }
                }

                // 实体化所有Set属性
                Action<DbContext> initializer = context => { foreach (var initer in initDelegates) { initer(context); } };
                var setInfo = new SetTypesInitializersPair(setTypeList, initializer);
                CacheList.Add(Key, setInfo);
                return setInfo;
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="contextKey">上下文类型</param>
        public static SetTypesInitializersPair Cache(Type contextKey)
        {
            return new ContextSetTypeCacheManger(contextKey).GetValue();
        }
    }
}