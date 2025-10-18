using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniHttpServer.Settings
{
    public class Singleton
    {
        private static Singleton instance;
        public JsonEntity Settings { get; private set; }

        private Singleton()
        {
            //Берем настройки из settings.json
            if (!File.Exists("settings.json"))
            {
                throw new Exception("Файла settings.json по пути settings.json не существует");
            }

            string jsonFromFile = File.ReadAllText("settings.json");
            JsonEntity? jsonEntity = JsonSerializer.Deserialize<JsonEntity>(jsonFromFile);

            if (jsonEntity == null)
            {
                throw new Exception("jsonObject пустой!");
            }

            Settings = jsonEntity;
        }


        public static Singleton GetInstance()
        {
            if (instance == null)
            {
                instance = new Singleton();
            }
            return instance;
        }
    }
}
