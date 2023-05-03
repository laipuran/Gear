using PuranLai.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using ProngedGear.Windows;
using Wpf.Ui.Controls;

namespace ProngedGear
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Dictionary<TimeOnly, string> _Data = new();
        Dictionary<TimeOnly, string> Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
                App.AppSettings.Mod_Timer = Data;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            EventDataGrid.ItemsSource = Data;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data.Add(new TimeOnly(
                    Parse.ParseFromString(HourTextBox.Text, 24).number,
                    Parse.ParseFromString(MinuteTextBox.Text, 60).number),
                    StringTextBox.Text);

                EventDataGrid.ItemsSource = null;
                EventDataGrid.ItemsSource = Data;
            }
            catch { }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Data = new();

            EventDataGrid.ItemsSource = null;
            EventDataGrid.ItemsSource = Data;
        }
    }
}
