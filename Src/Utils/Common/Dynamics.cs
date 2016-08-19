using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FS.Utils.Common
{
    /// <summary>
    ///     动态创建属性、方法
    /// </summary>
    public static class Dynamics
    {
        /// <summary>
        ///     动态程序集名称
        /// </summary>
        private static readonly AssemblyName AsmName = new AssemblyName("FS_Dynamic");

        /// <summary>
        ///     动态程序集生成器
        /// </summary>
        private static readonly AssemblyBuilder AsmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AsmName, AssemblyBuilderAccess.Run);

        /// <summary>
        ///     模块生成器
        /// </summary>
        private static readonly ModuleBuilder ModuleBuilder = AsmBuilder.DefineDynamicModule("FS_Dynamic_" + Guid.NewGuid());

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys">Key：属性名称；Value：属性类型</param>
        /// <param name="constructors">构造函数参数</param>
        /// <param name="baseType">继承的父类类型</param>
        public static Type CreateClassType(List<PropertyInfo> addPropertys, Type[] constructors = null, Type baseType = null)
        {
            //Check.IsTure(propertys == null || propertys.Count == 0, "propertys参数不能为空或为0");

            // 类名
            var className = Guid.NewGuid().ToString();
            // 类型生成器
            var typeBuilder = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, baseType);

            // 添加属性
            if (addPropertys != null && addPropertys.Count > 0) { foreach (var property in addPropertys) { var fieldBuilder = typeBuilder.DefineField(property.Name, property.PropertyType, FieldAttributes.Public); } }
            // 添加默认构造函数
            if (constructors != null && constructors.Length > 0)
            {
                IlGenerator(typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, null));
                IlGenerator(typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, constructors));
            }

            // 生成类型
            return typeBuilder.CreateType(); //ExpandoObject
        }

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys">Key：属性名称；Value：属性类型</param>
        /// <param name="baseType">继承的父类类型</param>
        public static Type CreateClassType(Dictionary<string, Type> addPropertys, Type baseType = null)
        {
            Check.IsTure(addPropertys == null || addPropertys.Count == 0, $"{nameof(addPropertys)}参数不能为空或为0");

            // 类名
            var className = Guid.NewGuid().ToString();
            // 类型生成器
            var typeBuilder = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, baseType);

            // 添加属性
            if (addPropertys != null && addPropertys.Count > 0)
            {
                foreach (var property in addPropertys) { typeBuilder.DefineField(property.Key, property.Value, FieldAttributes.Public); }
            }

            // 生成类型
            return typeBuilder.CreateType(); //ExpandoObject
        }

        // CodeDom生成 
        //public void CreateClass(Type defineType, string className, Dictionary<string, Type> dicProperty)
        //{
        //    // 命名空间
        //    var nameSpace = new CodeNamespace(defineType.Namespace);
        //    // 定义一个新类
        //    var sortDeleteClass = new CodeTypeDeclaration(className)
        //    {
        //        IsClass = true,
        //        Attributes = MemberAttributes.Public
        //    };
        //    foreach (var keyValue in dicProperty)
        //    {
        //        var field = new CodeMemberField(keyValue.Value, "_" + keyValue.Key) { Attributes = MemberAttributes.Private };
        //        var property = new CodeMemberProperty()
        //        {
        //            Name = keyValue.Key,
        //            Type = new CodeTypeReference(keyValue.Value),
        //            Attributes = MemberAttributes.Private,
        //        };
        //        // 定义Get
        //        property.GetStatements.Add(new CodeMethodReturnStatement(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
        //        // 定义Set
        //        property.SetStatements.Add(new CodeMethodReturnStatement(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), field.Name)));
        //        // 添加字段
        //        sortDeleteClass.Members.Add(field);
        //        // 添加属性
        //        sortDeleteClass.Members.Add(property);
        //    }
        //    nameSpace.Types.Add(sortDeleteClass);
        //    var cpr = CodeDomProvider.CreateProvider("C#");
        //    cpr.CompileAssemblyFromDom();
        //}

        /// <summary>
        ///     定义方法体
        /// </summary>
        /// <param name="ctor">构造函数生成器</param>
        private static void IlGenerator(ConstructorBuilder ctor)
        {
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg);
            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Stfld, myGreetingField);
            il.Emit(OpCodes.Ret);
        }
    }
}