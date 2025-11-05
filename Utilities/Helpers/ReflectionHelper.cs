using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class ReflectionHelper
    {
        public static object? GetNestedPropertyValue(object obj, string path)
        {
            foreach (var part in path.Split('.'))
            {
                if (obj == null) return null;
                var prop = obj.GetType().GetProperty(part);
                if (prop == null) return null;
                obj = prop.GetValue(obj);
            }

            return obj;
        }

        public static string PascalJoin(string root, string path)
        {
            var parts = path.Split('.');
            return root + string.Join("", parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));
        }
    }
}
