using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TerminalUi.Commands
{
    internal static class CommandInitializer
    {
        private const string FileFilter = "*.dll";
        private static readonly string ModsFullDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
        private static readonly StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;
        private static readonly StringComparer _comparer = StringComparer.InvariantCultureIgnoreCase;
        private static readonly HashSet<string> _loadedAssemblyNames = new(_comparer);

        static CommandInitializer()
        {
            // Subscribe to assembly resolution events (for dependency lookup)
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public static void InitCommands()
        {
            if (!Directory.Exists(ModsFullDir))
            {
                Directory.CreateDirectory(ModsFullDir);
                return;
            }

            foreach (var dllPath in Directory.GetFiles(ModsFullDir, FileFilter, SearchOption.AllDirectories))
            {
                try
                {
                    // Attempt to load assembly
                    var assembly = LoadAssemblySafely(dllPath);
                    if (assembly is null) continue;

                    // Registration command
                    var commandName = GetCommandNamesFromAssembly(assembly);
                    if (commandName.Any())
                        ExecuteInstructions.Register(assembly, commandName.ToArray());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load {dllPath}: {ex.Message}");
                }
            }
        }

        private static Assembly LoadAssemblySafely(string path)
        {
            AssemblyName assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(path);
            }
            catch
            {
                return null; // Not a valid assembly
            }

            var loaded = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.GetName().FullName.Equals(assemblyName.FullName, _comparison));
            if (loaded is not null) return loaded;

            // Avoid duplicate loading (based on full name)
            if (!_loadedAssemblyNames.Add(assemblyName.FullName)) return null;

            // Load using LoadFrom (which will trigger the AssemblyResolve event)
            return Assembly.LoadFrom(path);
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // First, locate the already loaded assemblies.
            var loaded = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.GetName().FullName.Equals(args.Name, _comparison));
            if (loaded is not null) return loaded;

            // Attempt to find matching DLLs in the mods directory and its subdirectories.
            AssemblyName assemblyName = new(args.Name);
            if (Directory.Exists(ModsFullDir))
            {
                foreach (var dllPath in Directory.GetFiles(ModsFullDir, FileFilter, SearchOption.AllDirectories))
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(dllPath);
                        if (name.FullName.Equals(assemblyName.FullName, _comparison))
                            return Assembly.LoadFrom(dllPath);
                    }
                    catch { /* Ignore invalid files */ }
                }
            }

            return null; // Not found
        }

        private static IEnumerable<string> GetCommandNamesFromAssembly(Assembly assembly) =>
            assembly.GetTypes()
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Select(x => x.GetCustomAttribute<TerminalCommandAttribute>())
                .Where(x => x is not null)
                .Select(x => x.Name)
                .Distinct(_comparer);
    }
}
