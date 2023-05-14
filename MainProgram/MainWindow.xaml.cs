using ModernWpf.Controls;
using ProngedGear.Views;
using ProngedGear.Windows;
using PuranLai.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
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
            WindowState = WindowState.Minimized;
            Opacity = 1;
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
    }
}
