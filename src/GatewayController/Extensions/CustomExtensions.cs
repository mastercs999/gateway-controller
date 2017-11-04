using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayController.Extensions
{
    public static class CustomExtensions
    {
        public static string GetRealTypeName(this Type type)
        {
            // We handle only generic types
            if (!type.IsGenericType)
                return type.Name;

            // First part is sanatized name
            StringBuilder sb = new StringBuilder();
            sb.Append(type.Name.Contains('`') ? type.Name.Substring(0, type.Name.IndexOf('`')) : type.Name);

            // Now append type arguments
            sb.Append('<');
            sb.Append(String.Join(",", type.GetGenericArguments().Select(x => x.GetRealTypeName())));
            sb.Append('>');

            return sb.ToString();
        }
    }
}
