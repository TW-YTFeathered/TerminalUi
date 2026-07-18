using Avalonia.Controls;
using System;
using TerminalUi.Commands;

namespace TerminalUi.UC;

public partial class ConsoleCommandOutputUC : UserControl
{
    private string _command;
    private string[] _args;

    public ConsoleCommandOutputUC(string input)
    {
        InitializeComponent();

        (_command, _args) = ExecuteInstructions.ParseCommand(input);

        Console.SetOutput(AppendText, () => _command);

        if (!string.IsNullOrWhiteSpace(input) &&
            ExecuteInstructions.Commands.TryGetValue(_command, out var act))
            act(_args);
    }

    public void AppendText(string text) =>
        DispalyContent.Text += text;


    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.ChangeUC(new ConsoleCommandInputUC());
    }
}
