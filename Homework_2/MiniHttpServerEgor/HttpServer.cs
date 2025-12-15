using MiniHttpServerEgorFramework.Settings;
using MiniHttpServerEgorFramework.Shared; 
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Server
{
    public class HttpServer
    {
        private HttpListener _listener;
        private JsonEntity _settings;

        public Action<HttpListenerContext>? Command { get; set; }
        public CancellationTokenSource cts = new CancellationTokenSource();

        public HttpServer(JsonEntity settings)
        {
            _listener = new HttpListener();
            _settings = settings;
        }

        public void Start()
        {
            string url = $"http://{_settings.Domain}:{_settings.Port}/";
            _listener.Prefixes.Add(url);
            _listener.Start();
            Console.WriteLine($"Сервер запущен на {url}");
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void Receive()
        {
            try
            {
                if (_listener.IsListening)
                {
                    _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
                }
            }
            catch (ObjectDisposedException)
            {
            
            }
        }

        private async void ListenerCallback(IAsyncResult ar)
        {
            if (!_listener.IsListening || cts.IsCancellationRequested)
            {
                return;
            }


            Receive();

            HttpListenerContext context;
            try
            {
                context = _listener.EndGetContext(ar);
            }
            catch (HttpListenerException)
            {
                return;
            }

            var request = context.Request;
            var response = context.Response;
            byte[]? responseBytes = null;
            string path = request.Url?.AbsolutePath?.Trim('/') ?? "";

            try
            {
                if (path.Equals("searcher", StringComparison.OrdinalIgnoreCase))
                {
                    string jsonResponse = "{\"status\": \"ok\", \"service\": \"Searcher\"}";
                    responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
                    response.ContentType = "application/json";
                }
                else if (path.Equals("chatgpt", StringComparison.OrdinalIgnoreCase))
                {
                    string jsonResponse = "{\"status\": \"ok\", \"service\": \"ChatGPT\"}";
                    responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
                    response.ContentType = "application/json";
                }
                //Обработка статических файлов
                else
                {
                    string searchPath = path.Length == 0 ? "Public/index.html" : path;

                    responseBytes = GetResponseBytes.Invoke(searchPath);

                    if (responseBytes != null)
                    {

                        response.ContentType = GetContentType.Invoke(searchPath);
                    }
                }

                if (responseBytes == null)
                {
                    response.StatusCode = 404;
                    string notFoundText = "Ошибка сервера. Страница не найдена";
                    responseBytes = Encoding.UTF8.GetBytes(notFoundText);
                    response.ContentType = "text/html; charset=utf-8";
                }

                // Отправка ответа
                response.ContentLength64 = responseBytes.Length;
                using Stream output = response.OutputStream;
                await output.WriteAsync(responseBytes, 0, responseBytes.Length);
                await output.FlushAsync();

                Console.WriteLine($"Запрос обработан: /{path}");
            }
            catch (Exception ex)
            {
                // Обработка любых непредвиденных ошибок при обработке запроса
                Console.WriteLine($"Критическая ошибка обработки запроса: {ex.Message}");
                response.StatusCode = 500;
                string errorText = "Внутренняя ошибка сервера";
                byte[] errorBytes = Encoding.UTF8.GetBytes(errorText);
                response.ContentLength64 = errorBytes.Length;

                try
                {
                    await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                }
                catch {}
            }
            finally
            {
                if (response.OutputStream != null)
                {
                    response.OutputStream.Close();
                }
            }
        }
    }
}