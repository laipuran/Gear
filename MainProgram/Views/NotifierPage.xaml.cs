using ProngedGear.Windows;
using PuranLai.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProngedGear.Views
{
    /// <summary>
    /// NotifierPage.xaml 的交互逻辑
    /// </summary>
    public partial class NotifierPage : Page
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
                App.AppSettings.Mod_Timing = Data;
            }
        }

        public NotifierPage()
        {
            InitializeComponent();

            var roller = App.AppSettings.RollerText;
            switch (roller.LoopMode)
            {
                case LoopMode.Shuffle:
                    ShuffleRadioButton.IsChecked = true;
                    break;
                case LoopMode.Normal:
                    NormalRadioButton.IsChecked = true;
                    break;
                default:
                    break;
            }
            FormulaToggleSwitch.IsOn = roller.DisplayMode == DisplayMode.Formula;
            ContentTextBox.Text = string.Join('\n', roller.Text);
            ApplyButton_Click(new(), new());

            Data = App.AppSettings.Mod_Timing;
            EventDataGrid.ItemsSource = Data;
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

        private void ClearMessageButton_Click(object sender, RoutedEventArgs e)
        {
            App.Notifier.ClearTexts();
        }
    }
}
