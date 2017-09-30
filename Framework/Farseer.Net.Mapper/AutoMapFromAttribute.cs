using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farseer.Net.Mapper
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
