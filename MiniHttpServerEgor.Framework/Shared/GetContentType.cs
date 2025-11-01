using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Shared
{
    public class GetContentType
    {
        public static string Invoke(string? path)
        {
            var extension = Path.GetExtension(path?.Trim('/'));

            return extension switch
            {
                ".html" => "text/html; charset=utf-8",
                ".css" => "text/css",
                ".json" => "text/json",
                ".png" => "image/png",
                ".js" => "text/javascript",
                ".webp" => "image/webp",
                ".ico" => "image/ico",
                ".scss" => "text/css",
                "" => "text/html",
                _ => "text"
            };
        }
    }
}
