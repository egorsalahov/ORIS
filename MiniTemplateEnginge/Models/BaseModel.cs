using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Models
{
    public abstract class BaseModel
    {
        public abstract int Start { get; set; }
        public abstract int Length { get; set; }
    }
}
