using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
