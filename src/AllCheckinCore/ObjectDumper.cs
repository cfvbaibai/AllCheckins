using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Core
{
    public static class ObjectDumper
    {
        public static string ShallowDumpProperties(object obj, int indentCount = 0)
        {
            if (obj == null)
            {
                return "<NULL>";
            }
            var indent = new string(' ', indentCount);
            var result = new StringBuilder();
            Type type = obj.GetType();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(obj);
                result.AppendFormat("{0}{1} = {2}", indent, name, value);
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
