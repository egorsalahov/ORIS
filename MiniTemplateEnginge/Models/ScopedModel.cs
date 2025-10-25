using MiniTemplateEnginge.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Models
{
    public class ScopedModel
    {
        private readonly object _parent;
        private readonly string _varName;
        private readonly object _value;

        public ScopedModel(object parent, string varName, object value)
        {
            _parent = parent;
            _varName = varName;
            _value = value;
        }

        public bool TryGet(string name, out object value)
        {
            if (string.Equals(name, _varName, StringComparison.OrdinalIgnoreCase))
            {
                value = _value;
                return true;
            }

            value = Helper.GetPropertyValue(_parent, name);
            return value != null;
        }
    }
}
