using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farseer.Net.Mapper
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
