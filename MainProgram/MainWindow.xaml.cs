using System.Windows;

namespace Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Opacity = 0;
            App.Notifier.SetText($"事件：修改主窗口[透明度]为[{Opacity}]");
        }
    }
}
