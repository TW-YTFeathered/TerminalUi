using Avalonia.Controls;

namespace TerminalUi.UC;

public partial class ConsoleCommandOutputUC : UserControl
{
    public ConsoleCommandOutputUC()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.ChangeUC(new ConsoleCommandInputUC());
    }
}