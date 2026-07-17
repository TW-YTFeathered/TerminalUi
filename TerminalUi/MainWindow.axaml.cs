using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace TerminalUi
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            ChangeUC(new ConsoleCommandUC());
        }

        public static MainWindow Instance { get; private set;  }
        public void ChangeUC(UserControl uc, string title = "Termina Ui")
        {
            spDisplay.Children.Add(uc);
            this.Title = title;

            Dispatcher.UIThread.Post(() =>
            {
                var textBox = uc.FindControl<TextBox>("txtInput");
                textBox?.Focus();
            }, DispatcherPriority.Loaded);
        }
    }
}