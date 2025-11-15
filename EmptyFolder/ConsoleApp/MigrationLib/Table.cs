using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationLib
{
    public class Table
    {
        public string Name { get; set; } = "";
        public List<Column> Columns { get; set; } = new();
    }
}
