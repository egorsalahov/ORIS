using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Core.HttpResponse
{
    public interface IHttpResult
    {
        string Execute(HttpListenerContext context); //чтобы не писать в каждом методе в контроллере что мы отдаем html, на базе шаблонизатора
    }
}
