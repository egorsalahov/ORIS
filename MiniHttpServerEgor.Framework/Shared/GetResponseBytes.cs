using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Shared
{
    public class GetResponseBytes
    {
        public static byte[]? Invoke(string path)
        {
            if (Path.HasExtension(path.Trim('/')))
                return TryGetFile(path);
            else
                return TryGetFile(path + "index.html");
        }

        private static byte[]? TryGetFile(string path)
        {
            try
            {
                var targetPath = Path.Combine(path.Split("/"));

                string? found = Directory.EnumerateFiles("Public", $"{Path.GetFileName(path)}", SearchOption.AllDirectories)
                                        .FirstOrDefault(f => f.EndsWith(targetPath, StringComparison.OrdinalIgnoreCase));

                if (found == null)
                    throw new FileNotFoundException(path);

                return File.ReadAllBytes(found);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
                return null;
            }
        }
    }
}
