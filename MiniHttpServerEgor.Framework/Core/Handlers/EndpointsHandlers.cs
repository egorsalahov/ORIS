using MiniHttpServerEgorFramework.Core.Abstracts;
using MiniHttpServerEgorFramework.Core.Attributes;
using System.Net;
using System.Reflection;
using System.Text;

namespace MiniHttpServerEgorFramework.Core.Handlers
{
    internal class EndpointsHandler : Handler
    {
        public override async void HandleRequest(HttpListenerContext context)
        {
            if (true)
            {
                var request = context.Request;
                var endpointName = request.Url?.AbsolutePath.Split('/')[1]; ;

                var assembly = Assembly.GetEntryAssembly();
                var endpont = assembly.GetTypes()
                                       .Where(t => t.GetCustomAttribute<EndpointAttribute>() != null)
                                       .FirstOrDefault(end => IsCheckedNameEndpoint(end.Name, endpointName));

                if (endpont == null) return; // TODO: 

                var method = endpont.GetMethods().Where(t => t.GetCustomAttributes(true)
                            .Any(attr => attr.GetType().Name.Equals($"Http{context.Request.HttpMethod}",
                                                                    StringComparison.OrdinalIgnoreCase)))
                            .FirstOrDefault();

                if (method == null) return;  // TODO:            

                // ----------------------------------------------------
                bool isBaseEndpoint = endpont.Assembly.GetTypes()
                                       .Any(t => typeof(EndpointBase)
                                       .IsAssignableFrom(t) && !t.IsAbstract);

                var instanceEndpoint = Activator.CreateInstance(endpont);

                if (isBaseEndpoint)
                {
                    (instanceEndpoint as EndpointBase).SetContext(context);
                }

                // ----------------------------------------------------

                var result = method.Invoke(instanceEndpoint, null);

                if (result is string stringContent)
                {
                    await WriteResponseAsync(context.Response, stringContent);
                }
                else
                {
                    // TODO: Дописать или переделать логику
                    await WriteResponseAsync(context.Response, result?.ToString() ?? "Отправлены данные. Статус ОК");
                }

            }
            // передача запроса дальше по цепи при наличии в ней обработчиков
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private bool IsCheckedNameEndpoint(string endpointName, string className) =>
            endpointName.Equals(className, StringComparison.OrdinalIgnoreCase) ||
            endpointName.Equals($"{className}Endpoint", StringComparison.OrdinalIgnoreCase);


        private static async Task WriteResponseAsync(HttpListenerResponse response, string content) // object
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            // получаем поток ответа и пишем в него ответ
            response.ContentLength64 = buffer.Length;
            using Stream output = response.OutputStream;
            // отправляем данные
            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }

    }
}
