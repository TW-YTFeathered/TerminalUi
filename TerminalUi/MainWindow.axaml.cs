using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using TerminalUi.UC;

namespace TerminalUi
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            ChangeUC(new ConsoleCommandInputUC());
        }

        private void TitleBar_PointerPressed(object sender, PointerPressedEventArgs e) => BeginMoveDrag(e);

        public static MainWindow Instance { get; private set; }
        public void ChangeUC(UserControl uc, string title = "Termina Ui")
        {
            spDisplay.Children.Add(uc);
            TitleTextBlock.Text = title;

            Dispatcher.UIThread.Post(() =>
            {
                var textBox = uc.FindControl<TextBox>("InputText");
                textBox?.Focus();
            }, DispatcherPriority.Loaded);
        }
    }
}