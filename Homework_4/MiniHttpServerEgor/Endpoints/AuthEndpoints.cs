using MiniHttpServer.Core.Atributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServer.Endpoints
{
    [Endpoint]
    internal class AuthEndpoint
    {
        // Get /auth/
        [HttpGet]
        public string LoginPage()
        {
            return "index.html";
        }
    }
}
