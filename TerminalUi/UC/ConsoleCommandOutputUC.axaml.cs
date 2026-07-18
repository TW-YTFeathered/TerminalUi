using Avalonia.Controls;
using System;
using TerminalUi.Commands;

namespace TerminalUi.UC;

public partial class ConsoleCommandOutputUC : UserControl
{
    private string _command;

    public ConsoleCommandOutputUC(string cmd)
    {
        InitializeComponent();

        _command = cmd;

        Console.SetOutput(AppendText, () => _command);

        if (!string.IsNullOrWhiteSpace(cmd) &&
            ExecuteInstructions.Commands.TryGetValue(cmd, out var act))
            act();
    }

    public void AppendText(string text) =>
        DispalyContent.Text += text;


    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.ChangeUC(new ConsoleCommandInputUC());
    }
}
