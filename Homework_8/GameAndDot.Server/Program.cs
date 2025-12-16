using GameAndDot.Shared;
using System.Net;
using System.Net.Sockets;

ServerObject server = new ServerObject();// создаем сервер
await server.ListenAsync(); // запускаем сервер

