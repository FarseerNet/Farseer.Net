using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Farseer.Net.Modules;
using Farseer.Net.Reflection;

namespace Farseer.Net.Mapper
{
    /// <summary>
    /// AutoMap初始化模块
    /// </summary>
    public class AutoMapperModule : FarseerModule
    {
        private readonly ITypeFinder typeFinder;
        public AutoMapperModule(ITypeFinder _typeFinder)
        {
            typeFinder = _typeFinder;
        }

        /// <summary>
        /// 查找所有标注了AutoMap、AutoMapFrom以及AutoMapTo特性的类型，并完成他们之间的Map
        /// </summary>
        public override void PostInitialize()
        {
            var types = typeFinder.Find(type =>
                type.IsDefined(typeof(AutoMapAttribute)) ||
                type.IsDefined(typeof(AutoMapFromAttribute)) ||
                type.IsDefined(typeof(AutoMapToAttribute))
                );
            AutoMapperHelper.CreateMap(types);
        }
    }
}
