using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPut : Attribute
    {
        public string Route { get; }
        public HttpPut(string route)
        {
            Route = route;
        }

        public HttpPut() { }
    }
}
