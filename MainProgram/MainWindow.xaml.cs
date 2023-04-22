using System.Linq;
using System.Windows;
using Toolkit.Windows;

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
            WindowState = WindowState.Minimized;
            Opacity = 1;
            App.Notifier.EnqueueText($"事件：修改主窗口[状态]为[{WindowState}]");
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppSettings.AutoScroll = true;
            App.AppSettings.RollerText = new()
            {
                LoopMode = NormalRadioButton.IsChecked == true ? LoopMode.Normal : LoopMode.Shuffle,
                DisplayMode = FormulaToggleSwitch.IsOn ? DisplayMode.Formula : DisplayMode.Text,
                Text = ContentTextBox.Text.Trim().Split('\n').ToList()
            };
            App.Notifier.SetScroller(ContentTextBox.Text.Trim().Split('\n').ToList()
                , FormulaToggleSwitch.IsOn ? DisplayMode.Formula : DisplayMode.Text);
        }

        private void NormalRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            App.AppSettings.RollerText.LoopMode = LoopMode.Normal;
        }

        private void ShuffleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            App.AppSettings.RollerText.LoopMode = LoopMode.Shuffle;
        }
    }
}
