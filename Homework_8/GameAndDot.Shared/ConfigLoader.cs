using GameAndDot.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GameAndDot.Shared
{
    public static class ConfigLoader
    {
        public static AppConfig Load()
        {
            string path = "configure.json";

            if (!File.Exists(path))
                throw new FileNotFoundException("Не найден configure.json");

            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<AppConfig>(json);
        }
    }
}
