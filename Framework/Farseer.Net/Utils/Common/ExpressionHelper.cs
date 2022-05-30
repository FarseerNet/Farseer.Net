using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Collections.Pooled;
using FS.Cache;
using FS.Utils.Common.ExpressionVisitor;

namespace FS.Utils.Common
{
    /// <summary>
    ///     表达式树扩展
    /// </summary>
    public static class ExpressionHelper
    {
        #region 合并

        /// <summary>
        ///     合并NewExpression
        /// </summary>
        /// <param name="exps"> 要合并的NewExpression </param>
        public static Expression MergeNewExpression(params Expression[] exps)
        {
            // 获取表达式树中实体类型
            var parentType = new GetParamVisitor().VisitAndReturnFirst(exp: exps[0]).Type;

            // 取得所有PropertyInfo
            using var memberExpressions = new GetMemberVisitor().Visit(exps: exps);
            using var lst               = memberExpressions.Select(selector: o => (PropertyInfo)o.Member).ToPooledList();

            // 构造函数参数类型
            var lstPropertyType = lst.Select(selector: o => o.PropertyType).ToArray();

            // 根据取得的PropertyInfo列表，创建新类型
            var classType = DynamicsClassTypeCacheManger.Cache(addProperty: null, constructors: lstPropertyType, baseType: parentType);
            // 获取新类型的构造函数
            var constructor = classType.GetConstructor(types: lstPropertyType);

            // 创建构造函数的参数表达式数组
            var constructorParam = BuildConstructorsParameters(defindType: classType, lstPropertyType: lst);
            return Expression.New(constructor: constructor, arguments: constructorParam);
        }

        /// <summary>
        ///     创建类的所有字段对应的MemberAccess
        /// </summary>
        /// <param name="defindType"> 所属实体类型 </param>
        /// <param name="lstPropertyType"> </param>
        /// <returns> 构造函数的参数表达式数组 </returns>
        private static Expression[] BuildConstructorsParameters(Type defindType, IEnumerable<PropertyInfo> lstPropertyType)
        {
            var lambdaParam = Expression.Parameter(type: defindType, name: "o");
            return lstPropertyType.Select(selector: t => Expression.MakeMemberAccess(expression: lambdaParam, member: t)).Cast<Expression>().ToArray();
        }

        /// <summary>
        ///     合并赋值Expression
        /// </summary>
        /// <param name="exp"> NewExpression </param>
        /// <param name="val"> 赋值 </param>
        public static Expression MergeAssignExpression(Expression exp, object val)
        {
            var       v                 = Expression.Constant(value: val);
            using var lstAssign         = new PooledList<Expression>();
            using var memberExpressions = new GetMemberVisitor().Visit(exp);
            foreach (var propertyInfo in memberExpressions)
            {
                var u = Expression.AddAssign(left: propertyInfo, right: Expression.Convert(expression: v, type: propertyInfo.Type));
                lstAssign.Add(item: u);
            }

            return MergeBlockExpression(exps: lstAssign.ToArray());
        }

        /// <summary>
        ///     合并NewExpression
        /// </summary>
        /// <param name="exps"> 要合并的NewExpression </param>
        public static Expression MergeBlockExpression(params Expression[] exps)
        {
            using var lstExp = new PooledList<Expression>();
            foreach (var item in exps)
            {
                if (item == null) continue;
                using var pooledList = new GetBlockExpressionVisitor().Visit(exp: item);
                lstExp.AddRange(collection: pooledList);
            }

            if (lstExp.Count == 0) return null;
            return Expression.Block(expressions: lstExp.ToArray());
        }

        ///// <summary>
        ///// And 操作
        ///// </summary>
        ///// <param name="left">左树</param>
        ///// <param name="right">右树</param>
        //public static Expression MergeAndAlsoExpression(Expression left, Expression right)
        //{
        //    if (left == null) { return right; }
        //    if (right == null) { return left; }

        //    var leftLambda = left as LambdaExpression;
        //    var rightLambda = right as LambdaExpression;

        //    var leftParam = leftLambda.Parameters[0];
        //    var rightParam = rightLambda.Parameters[0];
        //    return Expression.Lambda(ReferenceEquals(leftParam, rightParam) ? Expression.AndAlso(leftLambda.Body, rightLambda.Body) : Expression.AndAlso(leftLambda.Body, Expression.Invoke(right, leftParam)), leftParam);
        //}

        /// <summary>
        ///     And 操作
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="left"> 左树 </param>
        /// <param name="right"> 右树 </param>
        public static Expression<Func<TEntity, bool>> MergeAndAlsoExpression<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            if (left  == null) return right;
            if (right == null) return left;

            var leftParam  = left.Parameters[index: 0];
            var rightParam = right.Parameters[index: 0];
            return Expression.Lambda<Func<TEntity, bool>>(body: ReferenceEquals(objA: leftParam, objB: rightParam) ? Expression.AndAlso(left: left.Body, right: right.Body) : Expression.AndAlso(left: left.Body, right: Expression.Invoke(expression: right, leftParam)), leftParam);
        }

        /// <summary>
        ///     OR 操作
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="left"> 左树 </param>
        /// <param name="right"> 右树 </param>
        public static Expression<Func<TEntity, bool>> MergeOrElseExpression<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            if (left == null) return right;
            var param = left.Parameters[index: 0];
            return Expression.Lambda<Func<TEntity, bool>>(body: ReferenceEquals(objA: param, objB: right.Parameters[index: 0]) ? Expression.OrElse(left: left.Body, right: right.Body) : Expression.OrElse(left: left.Body, right: Expression.Invoke(expression: right, param)), param);
        }

        #endregion

        #region 生成字段比较

        /// <summary>
        ///     动态创建一个 o.ID == value的表达式树
        /// </summary>
        /// <typeparam name="TEntity"> 实体 </typeparam>
        /// <param name="val"> 右值 </param>
        /// <param name="expType"> 匹配符号类型 </param>
        /// <param name="memberName"> 左边ID成员名称 </param>
        public static Expression<Func<TEntity, bool>> CreateBinaryExpression<TEntity>(object val, ExpressionType expType = ExpressionType.Equal, string memberName = "ID") where TEntity : class, new()
        {
            var oParam = Expression.Parameter(type: typeof(TEntity), name: "o");
            var left   = Expression.MakeMemberAccess(expression: oParam, member: typeof(TEntity).GetMember(name: memberName)[0]);
            return CreateBinaryExpression<TEntity>(val: val, memberName: left, expType: expType);
        }

        /// <summary>
        ///     动态创建一个 o.ID == value的表达式树
        /// </summary>
        /// <typeparam name="TEntity"> 实体 </typeparam>
        /// <param name="val"> 右值 </param>
        /// <param name="expType"> 匹配符号类型 </param>
        /// <param name="memberName"> 左边ID成员名称 </param>
        public static Expression<Func<TEntity, bool>> CreateBinaryExpression<TEntity>(object val, MemberExpression memberName, ExpressionType expType = ExpressionType.Equal) where TEntity : class, new()
        {
            var              oParam = new GetParamVisitor().VisitAndReturnFirst(exp: memberName);
            var              right  = Expression.Convert(expression: Expression.Constant(value: val), type: memberName.Type);
            BinaryExpression where  = null;
            switch (expType)
            {
                case ExpressionType.Equal:
                    where = Expression.Equal(left: memberName, right: right);
                    break;
                case ExpressionType.NotEqual:
                    where = Expression.NotEqual(left: memberName, right: right);
                    break;
                case ExpressionType.GreaterThan:
                    where = Expression.GreaterThan(left: memberName, right: right);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    where = Expression.GreaterThanOrEqual(left: memberName, right: right);
                    break;
                case ExpressionType.LessThan:
                    where = Expression.LessThan(left: memberName, right: right);
                    break;
                case ExpressionType.LessThanOrEqual:
                    where = Expression.LessThanOrEqual(left: memberName, right: right);
                    break;
            }

            return (Expression<Func<TEntity, bool>>)Expression.Lambda(body: where, oParam);
        }

        /// <summary>
        ///     动态创建一个 ID == value的表达式树
        /// </summary>
        /// <param name="val"> 右值 </param>
        /// <param name="memberName"> 左边ID成员名称 </param>
        public static Expression<Func<object, bool>> CreateBinaryExpression(object val, string memberName = "ID")
        {
            var oParam = Expression.Parameter(type: val.GetType(), name: memberName);
            return (Expression<Func<object, bool>>)Expression.Lambda(body: Expression.Assign(left: oParam, right: Expression.Constant(value: val)), oParam);
        }

        /// <summary>
        ///     动态创建字段
        /// </summary>
        /// <param name="memberType"> 左边ID成员类型 </param>
        /// <param name="memberName"> 左边ID成员名称 </param>
        public static Expression CreateParameter(string memberName, Type memberType) => Expression.Parameter(type: memberType, name: memberName);

        /// <summary>
        ///     动态创建一个 List.Contains(o.ID)的表达式树
        /// </summary>
        /// <typeparam name="TEntity"> 实体 </typeparam>
        /// <param name="val"> 右值 </param>
        /// <param name="memberName"> 左边ID成员名称 </param>
        public static Expression<Func<TEntity, bool>> CreateContainsBinaryExpression<TEntity>(object val, string memberName = "ID") where TEntity : class, new()
        {
            var oParam = Expression.Parameter(type: typeof(TEntity), name: "o");
            var left   = Expression.MakeMemberAccess(expression: oParam, member: typeof(TEntity).GetMember(name: memberName)[0]);
            return CreateContainsBinaryExpression<TEntity>(val: val, memberName: left);
        }

        /// <summary>
        ///     动态创建一个 List.Contains(o.ID)的表达式树
        /// </summary>
        /// <typeparam name="TEntity"> 实体 </typeparam>
        /// <param name="val"> 右值 </param>
        /// <param name="memberName"> 左边ID成员名称 </param>
        public static Expression<Func<TEntity, bool>> CreateContainsBinaryExpression<TEntity>(object val, MemberExpression memberName) where TEntity : class, new()
        {
            var oParam = new GetParamVisitor().VisitAndReturnFirst(exp: memberName);
            // 值类型
            var valType                                                                                                  = val.GetType();
            if (valType.GetTypeInfo().IsGenericType && valType.GetGenericTypeDefinition() != typeof(Nullable<>)) valType = valType.GetGenericArguments()[0];
            var fieldMember                                                                                              = memberName.Type != valType && memberName.Type.GetTypeInfo().IsGenericType && memberName.Type.GetGenericTypeDefinition() == typeof(Nullable<>) ? (Expression)Expression.Call(instance: memberName, method: memberName.Type.GetMethod(name: "GetValueOrDefault", types: new Type[] { })) : memberName;

            var right = Expression.Constant(value: val, type: val.GetType());
            var where = Expression.Call(instance: right, method: right.Type.GetMethod(name: "Contains"), fieldMember);
            return (Expression<Func<TEntity, bool>>)Expression.Lambda(body: where, oParam);
        }

        #endregion

        #region 反射代替委托

        #region 创建实例化

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <param name="type"> 类型 </param>
        /// <param name="args"> 构造函数的参数列表 </param>
        public static T CreateInstance<T>(Type type, params object[] args) where T : class
        {
            //根据参数列表返回参数类型数组
            var parameterTypes = args.Select(selector: c => c.GetType()).ToArray();
            return (T)CreateInstance(type: type, parameterTypes: parameterTypes)(arg: args);
        }

        /// <summary>
        ///     创建用来返回构造函数的委托
        /// </summary>
        /// <param name="type"> 对象类型 </param>
        /// <param name="parameterTypes"> 构造函数的参数类型数组 </param>
        public static Func<object[], object> CreateInstance(Type type, params Type[] parameterTypes)
        {
            //根据参数类型数组来获取构造函数
            var constructor = parameterTypes == null ? type.GetConstructor(types: new Type[0]) : type.GetConstructor(types: parameterTypes);
            Check.NotNull(value: constructor, parameterName: type.FullName + "：不存在该参数个数签名的构造函数");

            //构造函数参数值
            var lambdaParam = Expression.Parameter(type: typeof(object[]), name: "_args");

            //创建构造函数的参数表达式数组
            var constructorParam = parameterTypes == null ? null : BuildParameters(paramExp: lambdaParam, parameterTypes: parameterTypes);

            //创建构造函数表达式
            var newExp = Expression.New(constructor: constructor, arguments: constructorParam);

            //创建lambda表达式，返回构造函数
            var lambdaExp = Expression.Lambda<Func<object[], object>>(body: newExp, lambdaParam);

            return lambdaExp.Compile();
        }

        /// <summary>
        ///     根据类型数组和lambda表达式的参数，转化参数成相应类型
        /// </summary>
        /// <param name="parameterTypes"> 类型数组 </param>
        /// <param name="paramExp"> lambda表达式的参数表达式（参数是：object[]） </param>
        /// <returns> 构造函数的参数表达式数组 </returns>
        private static IEnumerable<UnaryExpression> BuildParameters(ParameterExpression paramExp, params Type[] parameterTypes)
        {
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                //从参数表达式（参数是：object[]）中取出参数
                var arg = Expression.ArrayIndex(array: paramExp, index: Expression.Constant(value: i));
                //把参数转化成指定类型
                var argCast = Expression.Convert(expression: arg, type: parameterTypes[i]);

                yield return argCast;
            }
        }

        #endregion

        #region SetValue

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="propertyInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Action<object, object> SetValue(PropertyInfo propertyInfo)
        {
            // 实体类
            var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
            // 要赋的值
            var valueParam = Expression.Parameter(type: typeof(object), name: "value");

            //((T)instance)
            var castInstanceExpression = Expression.Convert(expression: instanceParam, type: propertyInfo.DeclaringType);

            // (T)value
            var castValueExpression = Expression.Convert(expression: valueParam, type: propertyInfo.PropertyType);

            // 调用PropertySet方法
            var setter           = propertyInfo.GetSetMethod();
            var assignExpression = Expression.Call(instance: castInstanceExpression, method: setter, castValueExpression);

            var lambdaExpression = Expression.Lambda<Action<object, object>>(body: assignExpression, instanceParam, valueParam);
            return lambdaExpression.Compile();
        }

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="fieldInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Action<object, object> SetValue(FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
            var valueParam    = Expression.Parameter(type: typeof(object), name: "value");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(expression: instanceParam, type: fieldInfo.DeclaringType);
            // (T)value
            var castValueExpression = Expression.Convert(expression: valueParam, type: fieldInfo.FieldType);
            //((T)instance).Field
            var propertyProperty = Expression.Field(expression: castInstanceExpression, field: fieldInfo);
            //((T)instance).Field = value
            var assignExpression = Expression.Assign(left: propertyProperty, right: castValueExpression);
            var lambdaExpression = Expression.Lambda<Action<object, object>>(body: assignExpression, instanceParam, valueParam);
            return lambdaExpression.Compile();
        }

        #endregion

        #region GetValue

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="propertyInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Func<object, object> GetValue(PropertyInfo propertyInfo)
        {
            // 实体类
            var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(expression: instanceParam, type: propertyInfo.DeclaringType);

            // 调用PropertyGet方法
            var getter           = propertyInfo.GetGetMethod();
            var val              = Expression.Call(instance: castInstanceExpression, method: getter);
            var lambdaExpression = Expression.Lambda<Func<object, object>>(body: Expression.Convert(expression: val, type: typeof(object)), instanceParam);
            return lambdaExpression.Compile();
        }

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="fieldInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Func<object, object> GetValue(FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(expression: instanceParam, type: fieldInfo.DeclaringType);
            //((T)instance).Property
            var fieldProperty    = Expression.Field(expression: castInstanceExpression, field: fieldInfo);
            var val              = Expression.Convert(expression: fieldProperty, type: typeof(object));
            var lambdaExpression = Expression.Lambda<Func<object, object>>(body: Expression.Convert(expression: val, type: typeof(object)), instanceParam);
            return lambdaExpression.Compile();
        }

        /// <summary>
        ///     动态构造获取值委托（静态字段）
        /// </summary>
        /// <param name="fieldInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Func<object, object> GetStaticValue(FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(expression: instanceParam, type: fieldInfo.DeclaringType);
            //((T)instance).Property
            var fieldProperty    = Expression.Field(expression: null, field: fieldInfo);
            var val              = Expression.Convert(expression: fieldProperty, type: typeof(object));
            var lambdaExpression = Expression.Lambda<Func<object, object>>(body: Expression.Convert(expression: val, type: typeof(object)), instanceParam);
            return lambdaExpression.Compile();
        }

        #endregion

        #endregion
    }
}