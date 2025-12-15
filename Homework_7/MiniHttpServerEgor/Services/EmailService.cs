
using MiniHttpServerEgorFramework.Settings.EmailSettings;
using System.Net;
using System.Net.Mail;

namespace MiniHttpServerEgor.Services
{
    public class EmailService
    {
        private readonly List<SmtpSettings> _smtpList;

        public EmailService()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");

            if (File.Exists(jsonPath))
            {
                var json = File.ReadAllText(jsonPath);

                // достаём только массив SmtpSettings из JSON
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var smtpJson = doc.RootElement.GetProperty("SmtpSettings").GetRawText();

                _smtpList = System.Text.Json.JsonSerializer.Deserialize<List<SmtpSettings>>(smtpJson)
                            ?? new List<SmtpSettings>();
            }
            else
            {
                Console.WriteLine("Файл settings.json не найден. Используется пустой список SMTP.");
                _smtpList = new List<SmtpSettings>();
            }
        }

        public async Task SendEmailAsync(string to, string title, string password)
        {
            foreach (var smtpSettings in _smtpList)
            {
                MailAddress from = new MailAddress(smtpSettings.Username, "Салахов Егор 11-409");
                MailAddress recepient = new MailAddress(to);

                MailMessage m = new MailMessage(from, recepient);
                m.Subject = title;
                m.Body = $@"
                            <html>
                                <body style='font-family: Arial, sans-serif; color: #333;'>
                                    <h2 style='color:#2e6c80;'>Здравствуйте!</h2>
                                    <p>Вы успешно авторизовались на сайте.</p>
                                    <p>Ваши данные для входа:</p>
                                    <ul>
                                        <li><b>Логин:</b> {to.ToString()}</li>
                                        <li><b>Пароль:</b> {password.ToString()}</li>
                                    </ul>
                                </body>
                            </html>";
                m.IsBodyHtml = true;

                
                var path = Path.Combine(Directory.GetCurrentDirectory(), "HomeWorkForEmail.zip"); 
                m.Attachments.Add(new Attachment(path));

                SmtpClient smtp = new SmtpClient(smtpSettings.Host, smtpSettings.Port);

                Console.WriteLine($"Подключение к {smtpSettings.Host}:{smtpSettings.Port}...");
                smtp.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);


                smtp.EnableSsl = smtpSettings.EnableSsl;
                Console.WriteLine("Настройки применены, пробую отправить письмо...");

                try
                {
                   await smtp.SendMailAsync(m);
                    Console.WriteLine($"Письмо отправлено через {smtpSettings.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке через {smtpSettings.Name}: {ex.Message}");
                }
                Console.WriteLine($"Письмо отправлено через {smtpSettings.Name}");
                //return;
            }
        }
    }
}
