using MiniHttpServerEgor.Core.Attributes;
using MiniHttpServerEgor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgor.Endpoints
{
    [Endpoint]
    internal class AuthEndpoint
    {
        private readonly EmailService emailService = new EmailService();

        // Get /auth/
        [HttpGet]
        public string LoginPage()
        {
            return "index.html";
        }

        // Post /auth/
        [HttpPost("/auth")]
        public async Task Login(string  email, string password)
        {
            // Отправка на почту email указанного email и password
            await emailService.SendEmailAsync(email, "Авторизация прошла успешно", password);
        }


        // Post /auth/sendEmail
        [HttpPost("sendEmail")]
        public void SendEmail(string to, string title, string message)
        {
            // Отправка на почту email указанного email и password


        }

    }
}
