using System;

namespace FS.Mapper
{
    public class AutoMapToAttribute : AutoMapAttribute
    {
        public AutoMapToAttribute(params Type[] targetTypes) : base(targetTypes) { }

        internal override AutoMapDirection Direction
        {
            get
            {
                return AutoMapDirection.To;
            }
        }
    }
}
