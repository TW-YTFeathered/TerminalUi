using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace TerminalUi.UC;

public partial class ConsoleCommandInputUC : UserControl
{
    public ConsoleCommandInputUC()
    {
        InitializeComponent();

        InputText.AddHandler(
            InputElement.KeyDownEvent,
            InputText_KeyDown,
            RoutingStrategies.Tunnel
        );
    }

    private void InputText_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            InputText.IsReadOnly = true;
            InputText.Focusable = false;
            InputText.IsHitTestVisible = false;

            MainWindow.Instance.ChangeUC(new ConsoleCommandOutputUC(InputText.Text));
        }
    }
}
