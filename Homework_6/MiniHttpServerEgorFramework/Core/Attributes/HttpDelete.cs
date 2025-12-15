using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDelete : Attribute
    {
        public string Route { get; }
        public HttpDelete(string route)
        {
            Route = route;
        }

        public HttpDelete() { }
    }
}
