using MiniHttpServerEgor;
using MiniHttpServerEgorFramework.Server;
using MiniHttpServerEgorFramework.Settings;
using MiniHttpServerEgorFramework.Shared;
using System.Text;



Singleton singleton = Singleton.GetInstance();
JsonEntity settings = singleton.Settings;

HttpServer server = new HttpServer(settings);

server.Command = async (context) =>
{

    //Формируем ответ
    var response = context.Response;

    //смотрим url запросика, конкретно абсолютный путь для удобства нашего на сервере
    string? path = context.Request.Url?.AbsolutePath;

    //сюда байты ответика будем записывать
    byte[]? responseBytes;

    if (path == null || path == "/")
        responseBytes = GetResponseBytes.Invoke($"Public/index.html");
    else
    {
        responseBytes = GetResponseBytes.Invoke($"{path}");
    }

    if (responseBytes == null)
    {
        response.OutputStream.Close();
    }
    else
    {
        response.ContentType = GetContentType.Invoke(path);

        response.ContentLength64 = responseBytes.Length;

        using Stream output = response.OutputStream;

        await output.WriteAsync(responseBytes);
        await output.FlushAsync();
    }
};


Task.Run(() =>
{
    server.Start();
});

while (true)
{
    if (Console.ReadLine() == "/stop")
    {
        server.cts.Cancel();
        server.Stop();
        break;
    }
}