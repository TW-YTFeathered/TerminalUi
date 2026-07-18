using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TerminalUi.Commands
{
    static class ExecuteInstructions
    {
        public static readonly Dictionary<string, Action> Commands = new(StringComparer.InvariantCultureIgnoreCase)
        {
            ["help"] = Help
        };

        public static void Help()
        {
            foreach (var cmd in Commands)
                Console.WriteLine(cmd.Key);
        }

        public static void Register(Assembly assembly, params string[] commands)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (commands == null || commands.Length == 0)
                throw new ArgumentException("At least one command name must be specified.", nameof(commands));

            // Create a filter list (remove whitespace and ignore case)
            var filter = commands
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            if (filter.Count == 0)
                throw new ArgumentException("All command names must be blank or null.", nameof(commands));

            // Scan all public static, parameterless, void postback methods in the component that have the TerminalCommandAttribute attribute.
            var methods = assembly.GetTypes()
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(x => x.ReturnType == typeof(void) && x.GetParameters().Length == 0)
                .Select(x => new
                {
                    Method = x,
                    Attr = x.GetCustomAttribute<TerminalCommandAttribute>()
                })
                .Where(x => x.Attr != null);

            foreach (var item in methods)
            {
                string cmdName = item.Attr.Name.Trim();

                // Only register commands specified in the commands list
                if (!filter.Contains(cmdName))
                    continue;

                // If the command name already exists, skip this step (you can choose to overwrite or throw an exception instead).
                if (!Commands.ContainsKey(cmdName))
                {
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), item.Method);
                    Commands[cmdName] = action;
                }
            }
        }
    }
}
