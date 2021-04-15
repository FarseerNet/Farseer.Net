using System.Reflection;

namespace FS.ElasticSearch.Internal
{
    /// <summary>
    ///     实体类包裹器初始化
    /// </summary>
    public static class SetInitializer
    {
        public static readonly MethodInfo IndexSetMethod = typeof (EsContext).GetMethod("IndexSet", new[] {typeof (PropertyInfo)});
    }
}