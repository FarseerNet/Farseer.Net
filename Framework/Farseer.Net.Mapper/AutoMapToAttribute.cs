using System;

namespace FS.Mapper
{
    public class AutoMapToAttribute : AutoMapAttribute
    {
        public AutoMapToAttribute(params Type[] targetTypes) : base(targetTypes) { }

        internal override EumAutoMapDirection Direction => EumAutoMapDirection.To;
    }
}
