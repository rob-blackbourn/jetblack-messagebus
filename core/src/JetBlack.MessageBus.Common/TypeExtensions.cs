#nullable enable

using System;
using System.Linq;

namespace JetBlack.MessageBus.Common
{
    public static class TypeExtensions
    {
        public static Type LoadType(this string fullTypeName)
        {
            var type = Type.GetType(fullTypeName);
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName == type.Assembly.FullName))
                AppDomain.CurrentDomain.Load(type.Assembly.FullName);
            return type;
        }

        public static T Construct<T>(this Type type, params object[] args)
        {
            return (T)Activator.CreateInstance(type, new[] { args });
        }
    }
}