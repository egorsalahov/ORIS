
using MiniHttpServerEgorFramework.Core.Abstracts;
using MiniHttpServerEgorFramework.Core.Handlers;
using MiniHttpServerEgorFramework.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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

            //Добавляем Prefixes
            string url = $"http://{_settings.Domain}:{_settings.Port}/"; //ТУТ БЫЛ НАИЛЬ
            _listener.Prefixes.Add(url);

            //Запускаем сервер
            _listener.Start();
            Console.WriteLine($"Сервер запущен на {url}");


            //Слушаем запросы (метод)
            Receive();


        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
        }


        private void ListenerCallback(IAsyncResult ar)
        {
            if (_listener.IsListening)
            {
                var context = _listener.EndGetContext(ar);

                Handler staticFilesHandler = new StaticFilesHandler();
                Handler endpointsHandler = new EndpointsHandler();
                staticFilesHandler.Successor = endpointsHandler;
                staticFilesHandler.HandleRequest(context);

                Receive();
            }
        }
    }
}
