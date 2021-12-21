using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FS.Cache;
using FS.Data.Internal;

namespace FS.Data.Cache
{
    /// <summary>
    ///     保存派生Context的Set类型
    /// </summary>
    internal class ContextSetTypeCacheManger : AbsCacheManger<Type, SetTypesInitializersPair>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new();

        private ContextSetTypeCacheManger(Type key) : base(key: key)
        {
        }

        protected override SetTypesInitializersPair SetCacheLock()
        {
            if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];
            lock (LockObject)
            {
                if (CacheList.ContainsKey(key: Key)) return CacheList[key: Key];

                var dbContextParam = Expression.Parameter(type: typeof(DbContext), name: "_context");

                var initDelegates = new List<Action<DbContext>>();

                // 一个DbContext对象下的所有Set实体类型
                var setTypeList = new Dictionary<Type, List<string>>();

                // 取得所有Set属性
                foreach (var entityMap in ContextMapCacheManger.Cache(key: Key).EntityMapList)
                {
                    // 实体类型，Set的泛型类型
                    var entityType = entityMap.Value != null ? entityMap.Key.PropertyType.GetGenericArguments()[0] : null;

                    // 查找这个Set类在当前DbContext中，出现过几次，并记录属性。
                    if (!setTypeList.TryGetValue(key: entityMap.Key.PropertyType, value: out var propertyName))
                    {
                        propertyName                                 = new List<string>();
                        setTypeList[key: entityMap.Key.PropertyType] = propertyName;
                    }

                    propertyName.Add(item: entityMap.Key.Name);

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
                        var newExpression = Expression.Call(instance: dbContextParam, method: setMethod, Expression.Constant(value: entityMap.Key));

                        // 赋值
                        var setExpression = Expression.Call(instance: Expression.Convert(expression: dbContextParam, type: Key), method: setter, newExpression);
                        initDelegates.Add(item: Expression.Lambda<Action<DbContext>>(body: setExpression, dbContextParam).Compile());
                    }
                }

                // 实体化所有Set属性
                Action<DbContext> initializer = context =>
                {
                    foreach (var initer in initDelegates) initer(obj: context);
                };
                var setInfo = new SetTypesInitializersPair(entityTypeToPropertyNameMap: setTypeList, setsInitializer: initializer);
                CacheList.Add(key: Key, value: setInfo);
                return setInfo;
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="contextKey"> 上下文类型 </param>
        public static SetTypesInitializersPair Cache(Type contextKey) => new ContextSetTypeCacheManger(key: contextKey).GetValue();
    }
}