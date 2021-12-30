using System.Reflection;
using FS.Modules;
using FS.Reflection;

namespace FS.Mapper
{
    /// <summary>
    ///     AutoMap初始化模块
    /// </summary>
    public class MapperModule : FarseerModule
    {
        private readonly ITypeFinder _typeFinder;

        public MapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        /// <summary>
        ///     查找所有标注了AutoMap、AutoMapFrom以及AutoMapTo特性的类型，并完成他们之间的Map
        /// </summary>
        public override void PreInitialize()
        {
            var types = _typeFinder.Find(predicate: type =>
                                             type.IsDefined(attributeType: typeof(MapAttribute))     ||
                                             type.IsDefined(attributeType: typeof(MapFromAttribute)) ||
                                             type.IsDefined(attributeType: typeof(MapToAttribute))
                                        );
            MapperHelper.CreateMap(typeArr: types);
        }
    }
}