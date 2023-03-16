using Toolkit.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PPTClassifier Classifier = new();
        public MainWindow()
        {
            InitializeComponent();
            Classifier.Show();
            ClassifyToggleButton.IsOn = true;
            if (ClassifyToggleButton.IsOn)
            {
                Classifier.Visibility = Visibility.Visible;
                Dispatcher.BeginInvoke(() =>
                {
                    this.Visibility = Visibility.Collapsed;
                });
            }
            else
                Classifier.Visibility = Visibility.Collapsed;
        }

        private void ClassifyToggleButton_Toggled(object sender, RoutedEventArgs e)
        {
            if (ClassifyToggleButton.IsOn)
            {
                Classifier.Visibility = Visibility.Visible;
            }
            else
                Classifier.Visibility = Visibility.Collapsed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
