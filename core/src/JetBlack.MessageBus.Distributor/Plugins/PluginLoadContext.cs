#nullable enable

using System;
using System.Reflection;
using System.Runtime.Loader;

namespace JetBlack.MessageBus.Distributor.Plugins
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(Environment.ExpandEnvironmentVariables(pluginPath));
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath == null)
                return null;

            var assembly = LoadFromAssemblyPath(assemblyPath);
            return assembly;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath == null)
                return IntPtr.Zero;

            var unmanagedDll = LoadUnmanagedDllFromPath(libraryPath);
            return unmanagedDll;
        }
    }
}
