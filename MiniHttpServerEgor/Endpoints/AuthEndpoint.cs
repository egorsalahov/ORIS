
using MiniHttpServerEgor.Services;
using MiniHttpServerEgorFramework.Core.Attributes;
using MiniHttpServerEgorFramework.Core;
using MiniHttpServerEgorFramework.Core.HttpResponse;

namespace MiniHttpServerEgor.Endpoints
{
    [Endpoint]
    public class AuthEndpoint : EndpointBase
    {
        // Get /auth/login
        [HttpGet]
        public IHttpResult LoginPage()
        {
            var obj = new { };

            return Page("index.html", obj);
        }

        // Get /auth/json
        [HttpGet("json")]
        public IHttpResult GetJson()
        {
            var user = new { Name = "sfdsdf" };

            return null;//Json(user);

            // ответ  '{"username":"Борис","Age":23}'
        }


        // Post /auth/
        [HttpPost]
        public void Login(/*string email, string password*/)
        {
            // Отправка на почту email указанного email и password
            // EmailService.SendEmail(email, title, message);
        }


        // Post /auth/sendEmail
        [HttpPost("sendEmail")]
        public void SendEmail(string to, string title, string message)
        {
            // Отправка на почту email указанного email и password


        }

    }
}
