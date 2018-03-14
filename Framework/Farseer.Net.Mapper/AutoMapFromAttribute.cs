using System;

namespace FS.Mapper
{
    public class AutoMapFromAttribute : AutoMapAttribute
    {
        public AutoMapFromAttribute(params Type[] targetTypes) : base(targetTypes) { }

        internal override AutoMapDirection Direction
        {
            get
            {
                return AutoMapDirection.From;
            }
        }
    }
}
