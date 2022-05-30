using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Collections.Pooled;

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
        private static readonly AssemblyName AsmName = new(assemblyName: "Farseer_Dynamic");

        /// <summary>
        ///     动态程序集生成器
        /// </summary>
        //private static readonly AssemblyBuilder AsmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AsmName, AssemblyBuilderAccess.Run);
        private static readonly AssemblyBuilder AsmBuilder = AssemblyBuilder.DefineDynamicAssembly(name: AsmName, access: AssemblyBuilderAccess.Run);

        /// <summary>
        ///     模块生成器
        /// </summary>
        private static readonly ModuleBuilder ModuleBuilder = AsmBuilder.DefineDynamicModule(name: "FS_Dynamic_" + Guid.NewGuid());

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys"> Key：属性名称；Value：属性类型 </param>
        /// <param name="constructors"> 构造函数参数 </param>
        /// <param name="baseType"> 继承的父类类型 </param>
        public static Type CreateClassType(PooledList<PropertyInfo> addPropertys, Type[] constructors = null, Type baseType = null)
        {
            //Check.IsTure(propertys == null || propertys.Count == 0, "propertys参数不能为空或为0");

            // 类名
            var className = Guid.NewGuid().ToString();
            // 类型生成器
            var typeBuilder = ModuleBuilder.DefineType(name: className, attr: TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, parent: baseType);

            // 添加属性
            if (addPropertys != null && addPropertys.Count > 0)
            {
                foreach (var property in addPropertys)
                {
                    var fieldBuilder = typeBuilder.DefineField(fieldName: property.Name, type: property.PropertyType, attributes: FieldAttributes.Public);
                }
            }

            // 添加默认构造函数
            if (constructors != null && constructors.Length > 0)
            {
                IlGenerator(ctor: typeBuilder.DefineConstructor(attributes: MethodAttributes.Public, callingConvention: CallingConventions.Any, parameterTypes: null));
                IlGenerator(ctor: typeBuilder.DefineConstructor(attributes: MethodAttributes.Public, callingConvention: CallingConventions.Any, parameterTypes: constructors));
            }

            // 生成类型
            return typeBuilder.CreateTypeInfo().AsType(); //ExpandoObject
        }

        /// <summary>
        ///     创建动态类
        /// </summary>
        /// <param name="addPropertys"> Key：属性名称；Value：属性类型 </param>
        /// <param name="baseType"> 继承的父类类型 </param>
        public static Type CreateClassType(PooledDictionary<string, Type> addPropertys, Type baseType = null)
        {
            Check.IsTure(isTrue: addPropertys == null || addPropertys.Count == 0, parameterName: $"{nameof(addPropertys)}参数不能为空或为0");

            // 类名
            var className = Guid.NewGuid().ToString();
            // 类型生成器
            var typeBuilder = ModuleBuilder.DefineType(name: className, attr: TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, parent: baseType);

            // 添加属性
            if (addPropertys != null && addPropertys.Count > 0)
            {
                foreach (var property in addPropertys) typeBuilder.DefineField(fieldName: property.Key, type: property.Value, attributes: FieldAttributes.Public);
            }

            // 生成类型
            return typeBuilder.CreateTypeInfo().AsType(); //ExpandoObject
        }

        /// <summary>
        ///     定义方法体
        /// </summary>
        /// <param name="ctor"> 构造函数生成器 </param>
        private static void IlGenerator(ConstructorBuilder ctor)
        {
            var il = ctor.GetILGenerator();
            il.Emit(opcode: OpCodes.Ldarg);
            //il.Emit(OpCodes.Ldarg_0);
            //il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Stfld, myGreetingField);
            il.Emit(opcode: OpCodes.Ret);
        }
    }
}