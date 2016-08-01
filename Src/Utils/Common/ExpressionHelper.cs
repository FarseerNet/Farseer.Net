using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        /// <param name="exps">要合并的NewExpression</param>
        public static Expression MergeNewExpression(params Expression[] exps)
        {
            // 获取表达式树中实体类型
            var parentType = new GetParamVisitor().Visit(exps[0]).Select(o => o.Type).First();

            // 取得所有PropertyInfo
            var lst = new GetMemberVisitor().Visit(exps).Select(o => (PropertyInfo) o.Member);

            // 构造函数参数类型
            var lstPropertyType = lst.Select(o => o.PropertyType).ToArray();

            // 根据取得的PropertyInfo列表，创建新类型
            var classType = DynamicsClassTypeCacheManger.Cache(null, lstPropertyType, parentType);
            // 获取新类型的构造函数
            var constructor = classType.GetConstructor(lstPropertyType);

            // 创建构造函数的参数表达式数组
            var constructorParam = BuildConstructorsParameters(classType, lst);
            return Expression.New(constructor, constructorParam);
        }

        /// <summary>
        ///     创建类的所有字段对应的MemberAccess
        /// </summary>
        /// <param name="defindType">所属实体类型</param>
        /// <param name="lstPropertyType"></param>
        /// <returns>构造函数的参数表达式数组</returns>
        private static Expression[] BuildConstructorsParameters(Type defindType, IEnumerable<PropertyInfo> lstPropertyType)
        {
            var lambdaParam = Expression.Parameter(defindType, "o");
            return lstPropertyType.Select(t => Expression.MakeMemberAccess(lambdaParam, t)).Cast<Expression>().ToArray();
        }

        /// <summary>
        ///     合并赋值Expression
        /// </summary>
        /// <param name="exp">NewExpression</param>
        /// <param name="val">赋值</param>
        public static Expression MergeAssignExpression(Expression exp, object val)
        {
            var v = Expression.Constant(val);
            var lstAssign = new List<Expression> {};
            foreach (var propertyInfo in new GetMemberVisitor().Visit(exp))
            {
                var u = Expression.AddAssign(propertyInfo, Expression.Convert(v, propertyInfo.Type));
                lstAssign.Add(u);
            }
            return MergeBlockExpression(lstAssign.ToArray());
        }

        /// <summary>
        ///     合并NewExpression
        /// </summary>
        /// <param name="exps">要合并的NewExpression</param>
        public static Expression MergeBlockExpression(params Expression[] exps)
        {
            var lstExp = new List<Expression>();
            foreach (var item in exps)
            {
                if (item == null) { continue; }
                lstExp.AddRange(new GetBlockExpressionVisitor().Visit(item));
            }
            if (lstExp.Count == 0) { return null; }
            return Expression.Block(lstExp.ToArray());
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
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TEntity, bool>> MergeAndAlsoExpression<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            if (left == null) { return right; }
            if (right == null) { return left; }

            var leftParam = left.Parameters[0];
            var rightParam = right.Parameters[0];
            return Expression.Lambda<Func<TEntity, bool>>(ReferenceEquals(leftParam, rightParam) ? Expression.AndAlso(left.Body, right.Body) : Expression.AndAlso(left.Body, Expression.Invoke(right, leftParam)), leftParam);
        }

        /// <summary>
        ///     OR 操作
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TEntity, bool>> MergeOrElseExpression<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            if (left == null) { return right; }
            var param = left.Parameters[0];
            return Expression.Lambda<Func<TEntity, bool>>(ReferenceEquals(param, right.Parameters[0]) ? Expression.OrElse(left.Body, right.Body) : Expression.OrElse(left.Body, Expression.Invoke(right, param)), param);
        }

        #endregion

        #region 生成字段比较

        /// <summary>
        ///     动态创建一个 o.ID == value的表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="val">右值</param>
        /// <param name="expType">匹配符号类型</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<TEntity, bool>> CreateBinaryExpression<TEntity>(object val, ExpressionType expType = ExpressionType.Equal, string memberName = "ID") where TEntity : class, new()
        {
            var oParam = Expression.Parameter(typeof (TEntity), "o");
            var left = Expression.MakeMemberAccess(oParam, typeof (TEntity).GetMember(memberName)[0]);
            return CreateBinaryExpression<TEntity>(val, left, expType);
        }

        /// <summary>
        ///     动态创建一个 o.ID == value的表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="val">右值</param>
        /// <param name="expType">匹配符号类型</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<TEntity, bool>> CreateBinaryExpression<TEntity>(object val, MemberExpression memberName, ExpressionType expType = ExpressionType.Equal) where TEntity : class, new()
        {
            var oParam = new GetParamVisitor().Visit(memberName).FirstOrDefault();
            var right = Expression.Convert(Expression.Constant(val), memberName.Type);
            BinaryExpression where = null;
            switch (expType)
            {
                case ExpressionType.Equal:
                    where = Expression.Equal(memberName, right);
                    break;
                case ExpressionType.NotEqual:
                    where = Expression.NotEqual(memberName, right);
                    break;
                case ExpressionType.GreaterThan:
                    where = Expression.GreaterThan(memberName, right);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    where = Expression.GreaterThanOrEqual(memberName, right);
                    break;
                case ExpressionType.LessThan:
                    where = Expression.LessThan(memberName, right);
                    break;
                case ExpressionType.LessThanOrEqual:
                    where = Expression.LessThanOrEqual(memberName, right);
                    break;
            }
            return (Expression<Func<TEntity, bool>>) Expression.Lambda(where, oParam);
        }

        /// <summary>
        ///     动态创建一个 ID == value的表达式树
        /// </summary>
        /// <param name="val">右值</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<object, bool>> CreateBinaryExpression(object val, string memberName = "ID")
        {
            var oParam = Expression.Parameter(val.GetType(), memberName);
            return (Expression<Func<object, bool>>) Expression.Lambda(Expression.Assign(oParam, Expression.Constant(val)), oParam);
        }

        /// <summary>
        ///     动态创建字段
        /// </summary>
        /// <param name="memberType">左边ID成员类型</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression CreateParameter(string memberName, Type memberType)
        {
            return Expression.Parameter(memberType, memberName);
        }

        /// <summary>
        ///     动态创建一个 List.Contains(o.ID)的表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="val">右值</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<TEntity, bool>> CreateContainsBinaryExpression<TEntity>(object val, string memberName = "ID") where TEntity : class, new()
        {
            var oParam = Expression.Parameter(typeof (TEntity), "o");
            var left = Expression.MakeMemberAccess(oParam, typeof (TEntity).GetMember(memberName)[0]);
            return CreateContainsBinaryExpression<TEntity>(val, left);
        }

        /// <summary>
        ///     动态创建一个 List.Contains(o.ID)的表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="val">右值</param>
        /// <param name="memberName">左边ID成员名称</param>
        public static Expression<Func<TEntity, bool>> CreateContainsBinaryExpression<TEntity>(object val, MemberExpression memberName) where TEntity : class, new()
        {
            var oParam = new GetParamVisitor().Visit(memberName).FirstOrDefault();
            // 值类型
            var valType = val.GetType();
            if (valType.IsGenericType && valType.GetGenericTypeDefinition() != typeof (Nullable<>)) { valType = valType.GetGenericArguments()[0]; }
            var fieldMember = memberName.Type != valType && memberName.Type.IsGenericType && memberName.Type.GetGenericTypeDefinition() == typeof (Nullable<>) ? (Expression) Expression.Call(memberName, memberName.Type.GetMethod("GetValueOrDefault", new Type[] {})) : memberName;

            var right = Expression.Constant(val, val.GetType());
            var where = Expression.Call(right, right.Type.GetMethod("Contains"), fieldMember);
            return (Expression<Func<TEntity, bool>>) Expression.Lambda(where, oParam);
        }

        #endregion

        #region 反射代替委托

        #region 创建实例化

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">构造函数的参数列表</param>
        public static T CreateInstance<T>(Type type, params object[] args) where T : class
        {
            //根据参数列表返回参数类型数组
            var parameterTypes = args.Select(c => c.GetType()).ToArray();
            return (T) CreateInstance(type, parameterTypes)(args);
        }

        /// <summary>
        ///     创建用来返回构造函数的委托
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="parameterTypes">构造函数的参数类型数组</param>
        public static Func<object[], object> CreateInstance(Type type, params Type[] parameterTypes)
        {
            //根据参数类型数组来获取构造函数
            var constructor = parameterTypes == null ? type.GetConstructor(new Type[0]) : type.GetConstructor(parameterTypes);
            Check.NotNull(constructor, type.FullName + "：不存在该参数个数签名的构造函数");

            //构造函数参数值
            var lambdaParam = Expression.Parameter(typeof (object[]), "_args");

            //创建构造函数的参数表达式数组
            var constructorParam = parameterTypes == null ? null : BuildParameters(lambdaParam, parameterTypes);

            //创建构造函数表达式
            var newExp = Expression.New(constructor, constructorParam);

            //创建lambda表达式，返回构造函数
            var lambdaExp = Expression.Lambda<Func<object[], object>>(newExp, lambdaParam);

            return lambdaExp.Compile();
        }

        /// <summary>
        ///     根据类型数组和lambda表达式的参数，转化参数成相应类型
        /// </summary>
        /// <param name="parameterTypes">类型数组</param>
        /// <param name="paramExp">lambda表达式的参数表达式（参数是：object[]）</param>
        /// <returns>构造函数的参数表达式数组</returns>
        private static Expression[] BuildParameters(ParameterExpression paramExp, params Type[] parameterTypes)
        {
            var list = new List<Expression>();
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                //从参数表达式（参数是：object[]）中取出参数
                var arg = Expression.ArrayIndex(paramExp, Expression.Constant(i));
                //把参数转化成指定类型
                var argCast = Expression.Convert(arg, parameterTypes[i]);

                list.Add(argCast);
            }
            return list.ToArray();
        }

        #endregion

        #region SetValue

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Action<object, object> SetValue(PropertyInfo propertyInfo)
        {
            // 实体类
            var instanceParam = Expression.Parameter(typeof (object), "instance");
            // 要赋的值
            var valueParam = Expression.Parameter(typeof (object), "value");

            //((T)instance)
            var castInstanceExpression = Expression.Convert(instanceParam, propertyInfo.DeclaringType);

            // (T)value
            var castValueExpression = Expression.Convert(valueParam, propertyInfo.PropertyType);

            // 调用PropertySet方法
            var setter = propertyInfo.GetSetMethod();
            var assignExpression = Expression.Call(castInstanceExpression, setter, castValueExpression);

            var lambdaExpression = Expression.Lambda<Action<object, object>>(assignExpression, instanceParam, valueParam);
            return lambdaExpression.Compile();
        }

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="fieldInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Action<object, object> SetValue(FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(typeof (object), "instance");
            var valueParam = Expression.Parameter(typeof (object), "value");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(instanceParam, fieldInfo.DeclaringType);
            // (T)value
            var castValueExpression = Expression.Convert(valueParam, fieldInfo.FieldType);
            //((T)instance).Field
            var propertyProperty = Expression.Field(castInstanceExpression, fieldInfo);
            //((T)instance).Field = value
            var assignExpression = Expression.Assign(propertyProperty, castValueExpression);
            var lambdaExpression = Expression.Lambda<Action<object, object>>(assignExpression, instanceParam, valueParam);
            return lambdaExpression.Compile();
        }

        #endregion

        #region GetValue

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Func<object, object> GetValue(PropertyInfo propertyInfo)
        {
            // 实体类
            var instanceParam = Expression.Parameter(typeof (object), "instance");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(instanceParam, propertyInfo.DeclaringType);

            // 调用PropertyGet方法
            var getter = propertyInfo.GetGetMethod();
            var val = Expression.Call(castInstanceExpression, getter);
            var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(val, typeof (object)), instanceParam);
            return lambdaExpression.Compile();
        }

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Func<object, object> GetValue(FieldInfo propertyInfo)
        {
            var instanceParam = Expression.Parameter(typeof (object), "instance");
            //((T)instance)
            var castInstanceExpression = Expression.Convert(instanceParam, propertyInfo.DeclaringType);
            //((T)instance).Property
            var propertyProperty = Expression.Field(castInstanceExpression, propertyInfo);
            var val = Expression.Convert(propertyProperty, typeof (object));
            var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(val, typeof (object)), instanceParam);
            return lambdaExpression.Compile();
        }

        #endregion

        #endregion
    }
}