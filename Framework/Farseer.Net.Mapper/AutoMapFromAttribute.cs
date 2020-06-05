using System;

namespace FS.Mapper
{
    public class AutoMapFromAttribute : AutoMapAttribute
    {
        public AutoMapFromAttribute(params Type[] targetTypes) : base(targetTypes) { }

        internal override EumAutoMapDirection Direction => EumAutoMapDirection.From;
    }
}
