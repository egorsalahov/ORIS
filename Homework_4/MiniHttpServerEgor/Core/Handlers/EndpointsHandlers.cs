using MiniHttpServer.Core.Abstracts;
using MiniHttpServer.Core.Atributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniHttpServer.Endpoints;
using System.IO;

namespace MiniHttpServer.Core.Handlers
{
    class EndpointsHandlers : Handler
    {
        public override void HandleRequest(HttpListenerContext context)
        {
            if (true)
            {
                var request = context.Request;
                var response = context.Response;
                var parts = request.Url?.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                var endpointName = parts?.FirstOrDefault();
                if (string.IsNullOrEmpty(endpointName))
                {
                    Successor?.HandleRequest(context);
                    return;
                }

                var assembly = Assembly.GetExecutingAssembly();
                var endpoint = assembly.GetTypes()
                       .Where(t => t.GetCustomAttribute<EndpointAttribute>() != null)
                       .FirstOrDefault(end => IsCheckedEndpoint(end.Name, endpointName));

                if (endpoint == null)
                {
                    Successor?.HandleRequest(context);
                    return;
                }

                var httpMethodName = $"Http{context.Request.HttpMethod}";
                var method = endpoint.GetMethods()
                    .FirstOrDefault(t => t.GetCustomAttributes(true)
                        .Any(attr => attr.GetType().Name.Equals(httpMethodName, StringComparison.OrdinalIgnoreCase)));

                if (method == null)
                {
                    response.StatusCode = 405;
                    response.StatusDescription = "Method Not Allowed";
                    response.OutputStream.Close();
                    return;
                }
                string body = "";
                if (request.HasEntityBody)
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    body = reader.ReadToEnd();
                }

                var postParams = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(body))
                {
                    foreach (var pair in body.Split('&', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var kv = pair.Split('=', 2); 
                        if (kv.Length > 0 && !string.IsNullOrEmpty(kv[0]))
                        {
                            string key = WebUtility.UrlDecode(kv[0]);
                            string value = (kv.Length > 1) ? WebUtility.UrlDecode(kv[1]) : "";
                            postParams[key] = value;
                        }
                    }
                }
                var parameters = method.GetParameters()
                    .Select(p => postParams.ContainsKey(p.Name) ? postParams[p.Name] : null)
                    .ToArray();

                var ret = method.Invoke(Activator.CreateInstance(endpoint), parameters);

                return;
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }

        private bool IsCheckedEndpoint(string className, string endpointName) =>
            className.Equals(endpointName, StringComparison.OrdinalIgnoreCase)
            || className.Equals($"{endpointName}Endpoint", StringComparison.OrdinalIgnoreCase);
    }
}