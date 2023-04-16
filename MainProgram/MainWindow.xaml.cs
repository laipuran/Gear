using System;
using System.Collections.Generic;
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
            Opacity= 1;
            App.Notifier.EnqueueText($"事件：修改主窗口[状态]为[{WindowState}]");
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            App.Notifier.SetScroller(ContentTextBox.Text.Trim().Split('\n').ToList()
                , FormulaToggleSwitch.IsOn ? DisplayMode.Formula : DisplayMode.Text);
        }

        private void NormalRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Loop = LoopMode.Normal;
        }

        private void ShuffleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            App.Loop = LoopMode.Shuffle;
        }
    }
}
