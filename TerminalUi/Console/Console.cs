using System;
using TerminalUi.UC;

namespace TerminalUi
{
    public class Console
    {
        private static Action<string>? _outputAction;
        private static Func<string?>? _inputFunc;

        internal static void SetOutput(Action<string> output, Func<string?>? input = null)
        {
            _outputAction = output;
            _inputFunc = input;
        }

        public static void Write(string? message) => _outputAction?.Invoke(message ?? string.Empty);
        public static void Write(params string[]? messages)
        {
            if (messages is null || messages.Length == 0)
                return;
            Write(string.Concat(messages));
        }

        private static void WriteLineInternal(string text) => _outputAction?.Invoke(text + Environment.NewLine);
        public static void WriteLine() => WriteLineInternal(string.Empty);
        public static void WriteLine(string? message) => WriteLineInternal(message ?? string.Empty);
        public static void WriteLine(params string[]? messages)
        {
            if (messages is null || messages.Length == 0)
            {
                WriteLine();
                return;
            }
            WriteLine(string.Concat(messages));
        }

        public static string? ReadLine() => _inputFunc?.Invoke();
    }
}
