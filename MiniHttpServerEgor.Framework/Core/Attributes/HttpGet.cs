﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGet : Attribute
    {
        public string Route { get; }
        public HttpGet(string route)
        {
            Route = route;
        }

        public HttpGet() { }
    }
}
