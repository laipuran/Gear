﻿using ModernWpf.Controls;
using ProngedGear.Views;
using PuranLai.APIs;
using System.Threading.Tasks;
using System.Windows;

namespace ProngedGear
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.Navigate(typeof(NotifierPage));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = (NavigationViewItem)args.SelectedItem;

            switch (item.Tag)
            {
                case "Classifier":
                    ContentFrame.Navigate(typeof(ClassifierPage));
                    break;

                case "Notifier":
                    ContentFrame.Navigate(typeof(NotifierPage));
                    break;
            }
        }

        private void NavigationView_BackRequested(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }

        private void WeatherCommand_Excuted(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            Task.Run(() =>
            {
                var ip = WeatherApis.GetHostIp();
                if (ip is null) return;
                var information = WeatherApis.GetIpInformation(ip);
                if (information is null) return;
                var weather = WeatherApis.GetWeatherInformation(information.Adcode);
                if (weather is null) return;
                var life = weather.Lives[0];
                string display = $"天气：{life.Weather}   温度：{life.Temperature}℃   风力：{life.WindPower}";
                App.Notifier.EnqueueText(display);
            });
        }
    }
}
