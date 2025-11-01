﻿
using MiniHttpServerEgorFramework.Core.HttpResponse;
using System.Net;

namespace MiniHttpServerEgorFramework.Core
{
    public abstract class EndpointBase
    {
        protected HttpListenerContext Context { get; private set; }

        internal void SetContext(HttpListenerContext context)
        {
            Context = context;
        }

        protected IHttpResult Page(string pathTemplate, object data) => new PageResult (pathTemplate, data);
    }
}
