using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace TerminalUi;

public partial class ConsoleCommandUC : UserControl
{
    public ConsoleCommandUC()
    {
        InitializeComponent();

        txtInput.AddHandler(
            InputElement.KeyDownEvent,
            txtInput_KeyDown,
            RoutingStrategies.Tunnel
        );
    }

    public void txtInput_KeyDown(object sender, KeyEventArgs e) 
    { 
        if (e.Key == Key.Enter)
        {
            txtInput.IsReadOnly = true;
            MainWindow.Instance.ChangeUC(new ConsoleCommandUC());
        }
    }
}