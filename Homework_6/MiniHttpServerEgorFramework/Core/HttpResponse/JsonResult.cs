using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace MiniHttpServerEgorFramework.Core.HttpResponse
{
    public class JsonResult : IHttpResult
    {
        private readonly object _data;

        public JsonResult(object data)
        {
            _data = data;
        }

        public string Execute(HttpListenerContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = 200;

            string json = JsonSerializer.Serialize(_data);
            return json;
        }
    }
}
