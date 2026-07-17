using Avalonia.Controls;
using System;
using System.Collections.Generic;
using TerminalUi.UC;

namespace TerminalUi
{
    static class ExecuteInstructions
    {
        public static readonly Dictionary<string, Action> Commands = new(StringComparer.InvariantCultureIgnoreCase)
        {
            ["cls"] = Cls,
            ["clear"] = Cls,
            ["exit"] = Exit
        };

        private static void Cls()
        {
            var spDisplay = MainWindow.Instance.FindControl<StackPanel>("spDisplay");

            if (spDisplay != null)
            {
                spDisplay.Children.Clear();

                MainWindow.Instance.ChangeUC(new ConsoleCommandInputUC());
            }
        }

        private static void Exit()
        {
            MainWindow.Instance.Close();
        }
    }
}
