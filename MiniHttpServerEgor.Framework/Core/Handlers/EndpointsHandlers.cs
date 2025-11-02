using MiniHttpServerEgorFramework.Core.Abstracts;
using MiniHttpServerEgorFramework.Core.Attributes;
using MiniHttpServerEgorFramework.Core.HttpResponse;
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


                //Если наследует EndpointBase -> создаем intsance -> передаем context
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

                int parameterCount = method.GetParameters().Length;
                if(parameterCount == 0) //если метод без параметров
                {
                    var result = method.Invoke(instanceEndpoint, null);

                    if (result is PageResult pageResult)
                    {
                        string html = pageResult.Execute(context);
                        await WriteResponseAsync(context.Response, html);
                        return;
                    }
                    else if (result is JsonResult jsonResult)
                    {
                        string json = jsonResult.Execute(context);
                        await WriteResponseAsync(context.Response, json);
                        return;
                    }
                    else
                    {
                        await WriteResponseAsync(context.Response, result?.ToString() ?? "Отправлены данные. Статус ОК");
                        return;
                    }
                }
                else //если метод с параметрами
                {
                    // Получаем параметры из запроса
                    object[] methodParams = GetMethodParameters(method, context);

                    // Логируем для отладки
                    Console.WriteLine($"Вызываем метод {method.Name} с {parameterCount} параметрами");
                    for (int i = 0; i < parameterCount; i++)
                    {
                        var param = method.GetParameters()[i];
                        Console.WriteLine($"  {param.Name} = {methodParams[i] ?? "null"}");
                    }

                    // Вызываем метод с параметрами
                    var result = method.Invoke(instanceEndpoint, methodParams);

                    // Обрабатываем результат
                    if (result is PageResult pageResult)
                    {
                        string html = pageResult.Execute(context);
                        await WriteResponseAsync(context.Response, html);
                        return;
                    }
                    else if (result is JsonResult jsonResult)
                    {
                        string json = jsonResult.Execute(context);
                        await WriteResponseAsync(context.Response, json);
                        return;
                    }
                    else if (result is Task task)
                    {
                        await task.ConfigureAwait(false);

                        var resultProperty = task.GetType().GetProperty("Result");
                        if (resultProperty != null)
                        {
                            var taskResult = resultProperty.GetValue(task);

                            if (taskResult is PageResult page)
                            {
                                string html = page.Execute(context);
                                await WriteResponseAsync(context.Response, html);
                            }
                            else if (taskResult is JsonResult json)
                            {
                                string jsonContent = json.Execute(context);
                                await WriteResponseAsync(context.Response, jsonContent);
                            }
                            else
                            {
                                await WriteResponseAsync(context.Response, taskResult?.ToString() ?? "OK");
                            }
                        }
                        return;
                    }
                    else
                    {
                        await WriteResponseAsync(context.Response, result?.ToString() ?? "Отправлены данные. Статус ОК");
                        return;
                    }
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


        private static async Task WriteResponseAsync(HttpListenerResponse response, string content) // object (?) зачем...html и json я как строки отдаю, это ошибка? если да - а как иначе отдавать json?
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            // получаем поток ответа и пишем в него ответ
            response.ContentLength64 = buffer.Length;
            using Stream output = response.OutputStream;
            // отправляем данные
            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }

        //для обработки параметров в endpointах
        private object[] GetMethodParameters(MethodInfo method, HttpListenerContext context)
        {
            var parameters = method.GetParameters();
            var result = new object[parameters.Length];

            try
            {
                // Читаем тело запроса
                string rawBody = "";
                if (context.Request.HasEntityBody)
                {
                    using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                    rawBody = reader.ReadToEnd();

                    Console.WriteLine($"Form data: {rawBody}");
                }

                // Если есть данные в теле - парсим FormData
                if (!string.IsNullOrEmpty(rawBody))
                {
                    // Парсим данные формы (email=test@mail.com&password=123456)
                    var formData = ParseFormData(rawBody);

                    // Заполняем параметры метода значениями из формы
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        string paramName = parameters[i].Name;
                        if (formData.ContainsKey(paramName) && !string.IsNullOrEmpty(formData[paramName]))
                        {
                            // Конвертируем строку в нужный тип
                            result[i] = Convert.ChangeType(formData[paramName], parameters[i].ParameterType);
                        }
                    }

                    Console.WriteLine("Параметры получены из FormData");
                    return result;
                }

                Console.WriteLine("Тело запроса пустое");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения параметров: {ex.Message}");
            }

            return result;
        }

        // Метод для парсинки данных формы
        private Dictionary<string, string> ParseFormData(string formData)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Разбиваем на пары: email=test@mail.com&password=123456
            var pairs = formData.Split('&');

            foreach (var pair in pairs)
            {
                // Разбиваем каждую пару на ключ и значение
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    // Раскодируем URL-encoded значения
                    string key = Uri.UnescapeDataString(keyValue[0]);
                    string value = Uri.UnescapeDataString(keyValue[1]);

                    result[key] = value;
                }
            }

            return result;
        }

    }
}
