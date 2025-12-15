using MiniTemplateEnginge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniTemplateEnginge.Common
{
    public static class Helper
    {
        public static object? GetPropertyValue(object dataModel, string propertyPath)
        {
            if (dataModel == null || string.IsNullOrWhiteSpace(propertyPath))
                return null;

            var parts = propertyPath.Split('.');

            var current = dataModel;
            foreach (var part in parts)
            {
                if (current == null)
                    return null;

                var type = current.GetType();

                if (current is ScopedModel scope && scope.TryGet(part, out var scopedValue))
                {
                    current = scopedValue;
                    continue;
                }

                var prop = type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null)
                    return null;

                current = prop.GetValue(current);
            }

            return current;
        }

        public static bool IsBoolProperty(string condition, object dataModel)
        {
            var value = GetPropertyValue(dataModel, condition);
            if (value is bool b) return b;
            return false;
        }
    }
}
