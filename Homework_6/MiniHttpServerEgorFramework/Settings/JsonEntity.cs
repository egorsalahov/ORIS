using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniHttpServerEgorFramework.Settings
{
    public class JsonEntity
    {
        public string Domain { get; set; }
        public string Port { get; set; }

        public JsonEntity(string domain, string port)
        {
            Domain = domain;
            Port = port;
        }

        public JsonEntity()
        {

        }
    }
}
