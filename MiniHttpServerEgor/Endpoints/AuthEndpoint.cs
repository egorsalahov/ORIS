
using MiniHttpServerEgor.Services;
using MiniHttpServerEgorFramework.Core;
using MiniHttpServerEgorFramework.Core.Attributes;
using MiniHttpServerEgorFramework.Core.HttpResponse;
using System.Text.Json;

namespace MiniHttpServerEgor.Endpoints
{
    [Endpoint]
    public class AuthEndpoint : EndpointBase
    {
        // Get /auth/login
        [HttpGet]
        public IHttpResult LoginPage()
        {
            var obj = new {  };

            return Page("Template/Page/login.thtml", obj);
        }

        // Get /auth/json
        [HttpGet("json")]
        public IHttpResult GetJson()
        {
            var user = new { Name = "sfdsdf" };

            return Json(user);
        }


        // Post /auth/
        [HttpPost]
        public async Task<IHttpResult> Login(string email, string password)
        {
           
            // Отправка на почту email указанного email и password
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(email, "Авторизация прошла успешно", password);

            string json = "{\"result\":\"отправка сообщения на почту прошла успешно\"}";
            return Json(json);
            
        }


        // Post /auth/sendEmail
        [HttpPost("sendEmail")]
        public void SendEmail(string to, string title, string message)
        {
            // Отправка на почту email указанного email и password


        }

    }
}
