using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Core.HttpResponse
{
    public class PageResult : IHttpResult
    {
        private readonly string _pathTemplate;
        private readonly object _data;

        public PageResult(string pathTemplate, object data)
        {
            _pathTemplate = pathTemplate;
            _data = data;
        }

        public string Execute(HttpListenerContext context)
        {
            // context.Response.ContentType = "application/html";
            // context.Response.StatusCode = "200";
            // return TepmlatorEngine.GetByFile(pathTemplate, data)

            // TODO: доработать логику в EndpointHandler
            // TODO: вызов методов шаблонизатора
            // TODO: реализовать JsonResult
            // Создать проект с тестами для  MiniHttpServer.Framework.UnitTests
            // покрыть тестами класс HttpServer

            return String.Empty;
        }
    }   
}
