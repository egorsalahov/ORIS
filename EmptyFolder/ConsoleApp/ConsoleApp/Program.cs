using System.Reflection;
using ConsoleApp.Models;
using MigrationLib;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings("localhost", "1337");

            string connectionString =
                "Host=localhost;Port=5432;Database=Migration;Username=postgres;Password=Linov1365";
            Assembly modelsAssembly = typeof(User).Assembly;

            // Собираем сервис миграций
            var dbAdapter = new PostgresDatabaseAdapter(connectionString);
            var sqlGenerator = new PostgresSqlGenerator();
            var snapshotBuilder = new SnapshotBuilder();
            var migrationGenerator = new MigrationGenerator(sqlGenerator);

            var migrationService = new MigrationService(
                dbAdapter,
                migrationGenerator,
                snapshotBuilder,
                modelsAssembly);

            var server = new HttpServer(settings, migrationService);

            bool keepRunning = true;
            bool keepAlive = false;

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                keepRunning = false;
            };

            server.Start(ref keepAlive);

            Console.WriteLine("Команды: /start, /restart, /off, /stop, /help");

            while (true)
            {
            }
        }
    }
}
