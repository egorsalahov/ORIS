using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace GameAndDot.Shared.Models
{
    public class ClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();

        public string Username { get; private set; } = String.Empty;
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        public string Color { get; set; } = String.Empty;

        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            var stream = client.GetStream();
            // создаем StreamReader для чтения данных
            Reader = new StreamReader(stream);
            // создаем StreamWriter для отправки данных
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                while (true)
                {
                    string? jsonRequest = await Reader.ReadLineAsync();
                    var messageRequest = JsonSerializer.Deserialize<EventMessage>(jsonRequest);

                    switch (messageRequest.type)
                    {
                        case Enums.EventType.PlayerConnected:
                            // СОХРАНЯЕМ ИМЯ ОТ КЛИЕНТА
                            Username = messageRequest.Username;

                            Console.WriteLine($"{Username} вошел в чат с цветом {Color}");

                            var messageResponse = new EventMessage()
                            {
                                type = Shared.Enums.EventType.PlayerConnected,
                                Username = Username,
                                Id = Id,

                                Players = server.Clients.Select(c => new PlayerInfo
                                {
                                    Username = c.Username,
                                    Color = c.Color
                                }).ToList()
                            };

                            string jsonResponse = JsonSerializer.Serialize(messageResponse);
                            await server.BroadcastMessageAllAsync(jsonResponse);
                            break;

                        case Enums.EventType.PointPlaced:

                            // просто пробрасываем всем
                            string pointJson = JsonSerializer.Serialize(messageRequest);
                            await server.BroadcastMessageAllAsync(pointJson);

                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
