using System;
using System.Linq;
using System.Reflection;

using JetBlack.MessageBus.Distributor.Plugins;

namespace JetBlack.MessageBus.Distributor.Configuration
{
    public class PluginConfig
    {
        public string? AssemblyPath { get; set; }
        public string? AssemblyName { get; set; }
        public string? TypeName { get; set; }
        public string[]? Args { get; set; }

        public override string ToString() => $"{nameof(AssemblyPath)}=\"{AssemblyPath}\",{nameof(AssemblyName)}=\"{AssemblyName}\",{nameof(TypeName)}=\"{TypeName}\",{nameof(Args)}={string.Join(",", Args ?? new string[0])}";

        public T? Construct<T>() where T : class
        {
            var type = LoadType();
            if (type == null)
                throw new ApplicationException("Failed to get type");

            return (T?)Activator.CreateInstance(type, new[] { Args });
        }

        private Type? LoadType()
        {
            if (TypeName == null)
                throw new ArgumentNullException(nameof(TypeName), "The type name must be specified");
            if (AssemblyName == null)
                throw new ArgumentNullException(nameof(TypeName), "The assembly name must be specified");

            if (string.IsNullOrWhiteSpace(AssemblyPath))
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var assembly = assemblies.First(x => x.GetName().Name == AssemblyName);
                var type = assembly.GetType(TypeName);
                return type;
            }
            else
            {
                var loadContext = new PluginLoadContext(AssemblyPath);
                var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(AssemblyName));
                var type = assembly.GetType(TypeName);
                return type;
            }
        }
    }
}