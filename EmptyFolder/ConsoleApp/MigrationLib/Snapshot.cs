using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationLib
{
    public class Snapshot
    {
        public List<Table> Tables { get; set; } = new();
    }
}
