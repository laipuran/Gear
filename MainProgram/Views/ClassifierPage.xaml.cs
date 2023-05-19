using System.Collections.Generic;
using System.Windows.Controls;

namespace ProngedGear.Views
{
    /// <summary>
    /// ClassifierPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClassifierPage : Page
    {
        Dictionary<string, string> _Subjects = new();

        Dictionary<string, string> Subjects
        {
            get
            {
                return _Subjects;
            }
            set
            {
                App.AppSettings.Subjects = value;
                _Subjects = value;
            }
        }

        public ClassifierPage()
        {
            InitializeComponent();
            SubjectsDataGrid.ItemsSource = App.AppSettings.Subjects;
        }



        private void SubjectTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LinkTextBox.Text = $"D:/{SubjectTextBox.Text}";
        }
    }
}
