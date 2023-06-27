using System;
using System.Windows;

namespace Gear.Windows
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public bool Result = false;

        public MessageWindow(string question, string symbol = "")
        {
            InitializeComponent();
            ContentTextBlock.Text = question;
            SymbolTextBlock.Text = string.IsNullOrEmpty(symbol) ? Convert.ToChar(0xE11B).ToString() : symbol;
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
