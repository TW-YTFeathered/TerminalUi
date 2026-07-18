using System;

namespace TerminalUi.Commands
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TerminalCommandAttribute : Attribute
    {
        public string Name { get; }

        public TerminalCommandAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The command name cannot be null or blank.", nameof(name));
            Name = name;
        }
    }
}
