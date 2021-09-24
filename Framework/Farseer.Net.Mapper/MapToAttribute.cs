using System;

namespace FS.Mapper
{
    public class MapToAttribute : MapAttribute
    {
        public MapToAttribute(params Type[] targetTypes) : base(targetTypes: targetTypes)
        {
        }

        internal override EumMapDirection Direction => EumMapDirection.To;
    }
}