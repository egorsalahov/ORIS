using MiniHttpServer.Server;
using MiniHttpServer.Settings;
using System.Text;

Singleton singleton = Singleton.GetInstance();
JsonEntity settings = singleton.Settings;

HttpServer server = new HttpServer(settings);

server.Command = async (context) =>
{

    //Формируем ответ
    var response = context.Response;
    string? responseText = File.ReadAllText("Public/index.html");


    //Отправляем html в виде потока байтов
    byte[] buffer = Encoding.UTF8.GetBytes(responseText);

    response.ContentLength64 = buffer.Length;
    using Stream output = response.OutputStream;

    await output.WriteAsync(buffer);
    await output.FlushAsync();

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