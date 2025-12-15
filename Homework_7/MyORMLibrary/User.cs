using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary
{
    public class User
    {
        public int Age { get; set; }
        public string Login { get; set;}    
        public string Username { get; set; }
        public string Password { get; set;}

        public User()
        {
        }
    }
}
