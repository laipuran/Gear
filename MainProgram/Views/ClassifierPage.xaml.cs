using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Controls;

namespace ProngedGear.Views
{
    /// <summary>
    /// ClassifierPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClassifierPage : Page
    {
        Models.Subject.SchoolSubject[] _Subjects = new Models.Subject.SchoolSubject[6];

        Models.Subject.SchoolSubject[] Subjects
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
            Subjects = App.AppSettings.Subjects;

            var items = new List<string>
            {
                "语文",
                "数学",
                "英语",
                "物理",
                "化学",
                "生物",
                "政治",
                "历史",
                "地理",
            };

            ComboBox_1.ItemsSource = items;
            ComboBox_2.ItemsSource = items;
            ComboBox_3.ItemsSource = items;
            ComboBox_4.ItemsSource = items;
            ComboBox_5.ItemsSource = items;
            ComboBox_6.ItemsSource = items;
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string name = comboBox.Name;
            int index = PuranLai.Algorithms.Parse.ParseFromString(name[name.Length - 1].ToString(), 6).number;
            var schoolSubject = (Models.Subject.SchoolSubject)comboBox.SelectedIndex;
            Subjects[index] = schoolSubject;
            App.Classifier.SetButtonSubject(schoolSubject, index);
        }
    }
}
