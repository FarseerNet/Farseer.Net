using System.Linq;
using System.Reflection;

namespace Farseer.Net.Data.Internal
{
    /// <summary>
    ///     实体类包裹器初始化
    /// </summary>
    public static class SetInitializer
    {
        public static readonly MethodInfo TableSetMethod = typeof (DbContext).GetMethod("TableSet", new[] {typeof (PropertyInfo)});
        public static readonly MethodInfo TableSetCacheMethod = typeof (DbContext).GetMethod("TableSetCache", new[] {typeof (PropertyInfo)});
        public static readonly MethodInfo ViewSetMethod = typeof (DbContext).GetMethod("ViewSet", new[] {typeof (PropertyInfo)});
        public static readonly MethodInfo ViewSetCacheMethod = typeof (DbContext).GetMethod("ViewSetCache", new[] {typeof (PropertyInfo)});
        public static readonly MethodInfo ProcSetMethod = typeof (DbContext).GetMethod("ProcSet", new[] {typeof (PropertyInfo)});
        public static readonly MethodInfo SqlSetMethod = typeof (DbContext).GetMethods().Single(o => o.Name == "SqlSet" && o.GetParameters()[0].ParameterType == typeof (PropertyInfo) && o.IsGenericMethod);
        public static readonly MethodInfo SqlSetNonGenericMethod = typeof (DbContext).GetMethods().Single(o => o.Name == "SqlSet" && o.GetParameters()[0].ParameterType == typeof (PropertyInfo) && !o.IsGenericMethod);
    }
}