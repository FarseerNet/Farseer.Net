using System.Linq;
using System.Reflection;

namespace FS.Data.Internal
{
    /// <summary>
    ///     实体类包裹器初始化
    /// </summary>
    public static class SetInitializer
    {
        public static readonly MethodInfo TableSetMethod         = typeof(DbContext).GetMethod(name: "TableSet", types: new[] { typeof(PropertyInfo) });
        public static readonly MethodInfo TableSetCacheMethod    = typeof(DbContext).GetMethod(name: "TableSetCache", types: new[] { typeof(PropertyInfo) });
        public static readonly MethodInfo ViewSetMethod          = typeof(DbContext).GetMethod(name: "ViewSet", types: new[] { typeof(PropertyInfo) });
        public static readonly MethodInfo ViewSetCacheMethod     = typeof(DbContext).GetMethod(name: "ViewSetCache", types: new[] { typeof(PropertyInfo) });
        public static readonly MethodInfo ProcSetMethod          = typeof(DbContext).GetMethod(name: "ProcSet", types: new[] { typeof(PropertyInfo) });
        public static readonly MethodInfo SqlSetMethod           = typeof(DbContext).GetMethods().Single(predicate: o => o.Name == "SqlSet" && o.GetParameters()[0].ParameterType == typeof(PropertyInfo) && o.IsGenericMethod);
        public static readonly MethodInfo SqlSetNonGenericMethod = typeof(DbContext).GetMethods().Single(predicate: o => o.Name == "SqlSet" && o.GetParameters()[0].ParameterType == typeof(PropertyInfo) && !o.IsGenericMethod);
    }
}