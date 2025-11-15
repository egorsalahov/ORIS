using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationLib
{
    public class Column
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsPrimaryKey { get; set; }
    }
}
