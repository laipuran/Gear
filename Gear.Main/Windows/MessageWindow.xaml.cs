using System.Windows;

namespace Gear.Windows
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public bool Result = false;

        public MessageWindow(string question, string symbol = "\uE11B")
        {
            InitializeComponent();
            ContentTextBlock.Text = question;
            SymbolTextBlock.Text = symbol;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }
    }
}
