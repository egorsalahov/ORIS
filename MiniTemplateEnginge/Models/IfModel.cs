using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Models
{
    public class IfModel : BaseModel
    {
        public required Condition Condition { get; set; }
        public string IfContent { get; set; } = String.Empty;
        public string? ElseContent { get; set; }
        public override int Start { get; set; }
        public override int Length { get; set; }
    }
}
