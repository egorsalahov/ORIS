using GameAndDot.Shared.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace GameAndDot.Shared
{
    public class ServerObject
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888); // сервер для прослушивания
        public List<ClientObject> Clients { get; private set; } = new List<ClientObject>(); // все подключения, список пользователей

        private readonly List<string> AvailableColors = new List<string> {
            "Red", "Blue", "Green", "Yellow", "Purple", "Cyan"
        };

        //получение следующего цвета для следующего игрока
        private string GetNextColor()
        {
            int index = Clients.Count % AvailableColors.Count;
            return AvailableColors[index];
        }

        //отрубка игрока
        public void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject? client = Clients.FirstOrDefault(c => c.Id == id);

            // и удаляем его из списка подключений
            if (client != null) Clients.Remove(client);

            client?.Close();
        }

        // прослушивание входящих подключений (мы же сервер)
        public async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync(); //ожидаем подключение клиентов

                    ClientObject clientObject = new ClientObject(tcpClient, this);

                    clientObject.Color = GetNextColor();
                    Clients.Add(clientObject);

                    Task.Run(clientObject.ProcessAsync); //в отдельном потоке запускаем конкретно для этого нового пользователя метод, что он будет писать сообщения
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        //отправка действия всей подключенной братве
        public async Task BroadcastMessageAllAsync(string message)
        {
            foreach (var client in Clients)
            {
                
                    await client.Writer.WriteLineAsync(message); //передача данных
                    await client.Writer.FlushAsync();
                
            }
        }

        // отключение всей братвы
        public void Disconnect()
        {
            foreach (var client in Clients)
            {
                client.Close(); //отключение клиента
            }
            tcpListener.Stop(); //остановка сервера
        }
    }
}
