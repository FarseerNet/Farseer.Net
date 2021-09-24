using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FS.Utils.Component
{
    /// <summary>
    ///     继承IComparer接口，实现同一自定义类型　对象比较
    /// </summary>
    /// <typeparam name="T"> T为泛用类型 </typeparam>
    public class Reverser<T> : IComparer<T>
    {
        private readonly ReverserInfo info;
        private readonly Type         type;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="type"> 进行比较的类类型 </param>
        /// <param name="name"> 进行比较对象的属性名称 </param>
        /// <param name="orderBy"> 比较方向(升序/降序) </param>
        public Reverser(Type type, string name, ReverserInfo.eumOrderBy orderBy = ReverserInfo.eumOrderBy.Desc)
        {
            this.type = type;
            info.Name = name;
            if (orderBy != ReverserInfo.eumOrderBy.Asc) info.OrderBy = orderBy;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="className"> 进行比较的类名称 </param>
        /// <param name="name"> 进行比较对象的属性名称 </param>
        /// <param name="orderBy"> 比较方向(升序/降序) </param>
        public Reverser(string className, string name, ReverserInfo.eumOrderBy orderBy = ReverserInfo.eumOrderBy.Desc)
        {
            if (string.IsNullOrWhiteSpace(value: className) || string.IsNullOrWhiteSpace(value: name)) return;
            type         = Type.GetType(typeName: className, throwOnError: true);
            info.Name    = name;
            info.OrderBy = orderBy;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="t"> 进行比较的类型的实例 </param>
        /// <param name="name"> 进行比较对象的属性名称 </param>
        /// <param name="orderBy"> 比较方向(升序/降序) </param>
        public Reverser(T t, string name, ReverserInfo.eumOrderBy orderBy = ReverserInfo.eumOrderBy.Desc)
        {
            type         = t.GetType();
            info.Name    = name;
            info.OrderBy = orderBy;
        }

        //必须！实现IComparer<T>的比较方法。
        int IComparer<T>.Compare(T t1, T t2)
        {
            var x = type.InvokeMember(name: info.Name, invokeAttr: BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, binder: null, target: t1, args: null);
            var y = type.InvokeMember(name: info.Name, invokeAttr: BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, binder: null, target: t2, args: null);
            if (info.OrderBy != ReverserInfo.eumOrderBy.Asc) Swap(x: ref x, y: ref y);
            return new CaseInsensitiveComparer().Compare(a: x, b: y);
        }

        //交换操作数
        private void Swap(ref object x, ref object y)
        {
            object temp = null;
            temp = x;
            x    = y;
            y    = temp;
        }
    }

    /// <summary>
    ///     对象比较时使用的信息类
    /// </summary>
    public struct ReverserInfo
    {
        /// <summary>
        ///     比较的方向，如下：
        ///     ASC：升序
        ///     DESC：降序
        /// </summary>
        public enum eumOrderBy : byte
        {
            /// <summary>
            ///     升序
            /// </summary>
            Asc = 0,

            /// <summary>
            ///     降序
            /// </summary>
            Desc
        }

        /// <summary>
        ///     要反映序的字段名称
        /// </summary>
        public string Name;

        /// <summary>
        ///     排序方式
        /// </summary>
        public eumOrderBy OrderBy;
    }
}