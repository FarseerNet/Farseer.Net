using System;

namespace FS.Mapper
{
    public class MapFromAttribute : MapAttribute
    {
        public MapFromAttribute(params Type[] targetTypes) : base(targetTypes: targetTypes)
        {
        }

        internal override EumMapDirection Direction => EumMapDirection.From;
    }
}