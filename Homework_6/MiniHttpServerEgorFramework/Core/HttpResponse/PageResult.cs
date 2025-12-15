using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MiniTemplateEnginge.Core;

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

        //тут отдается html-код на основе шаблонизатора, для методов в контроллере
        public string Execute(HttpListenerContext context)
        {
             context.Response.ContentType = "application/html";
             context.Response.StatusCode = 200;
             context.Response.ContentType = "text/html; charset=utf-8";


            // TODO: реализовать JsonResult (это класс как вот этот PageResult, тоже наследуется от IHttpResult)

            // TODO: доработать логику в EndpointHandler (что возвращать если пришла string, что возвращать если пришло json)


            // Создать проект с тестами для  MiniHttpServerEgorFramework.UnitTests
            // покрыть тестами класс HttpServer

            HtmlTemplateRenderer render = new HtmlTemplateRenderer();
            return render.RenderFromFile(_pathTemplate, _data);
        }
    }   
}
