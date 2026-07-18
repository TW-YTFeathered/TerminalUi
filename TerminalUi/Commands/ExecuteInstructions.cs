using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TerminalUi.Commands
{
    static class ExecuteInstructions
    {
        public static readonly Dictionary<string, Action<string[]>> Commands = new(StringComparer.InvariantCultureIgnoreCase)
        {
            ["help"] = (_ => Help())
        };

        private static void Help()
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
                .Where(x => x.ReturnType == typeof(void) &&
                    (x.GetParameters().Length == 0 || 
                    (x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(string[]))))
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
                    var method = item.Method;
                    var parameters = method.GetParameters();
                    Action<string[]> action;

                    if (parameters.Length == 0)
                    {
                        // Parameterless method: Wrapped as an ignored parameter
                        var del = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        action = args => del();
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
                    {
                        // Methods with string[] parameters: direct binding
                        action = (Action<string[]>)Delegate.CreateDelegate(typeof(Action<string[]>), method);
                    }
                    else
                    {
                        // Other signatures are not supported; skip or log warning.
                        Debug.WriteLine($"The method {method.Name} signature is not supported (must be parameterless or have a string[] parameter).");
                        continue;
                    }

                    Commands[cmdName] = action;
                }
            }
        }

        public static (string command, string[] args) ParseCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (string.Empty, Array.Empty<string>());

            string cmd = GetCommand(input);
            var args = GetArgs(input);
            return (cmd, args);
        }

        private static string GetCommand(string input)
        {
            int spaceIndex = input.IndexOf(' ');
            return spaceIndex == -1 ? input : input.Substring(0, spaceIndex);
        }

        private static string[] GetArgs(string input)
        {
            List<string> args = new();
            StringBuilder current = new();
            bool inQuotes = false;
            bool escaped = false;

            foreach (var c in input)
            {
                if (escaped)
                {
                    current.Append(c);
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                    current.Append(c);
            }

            if (current.Length > 0)
                args.Add(current.ToString());

            return args.Skip(1).ToArray();
        }
    }
}
