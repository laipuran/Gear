using System.Collections.Generic;
using System.Windows.Controls;

namespace ProngedGear.Views
{
    /// <summary>
    /// ClassifierPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClassifierPage : Page
    {
        Models.Subject[] _Subjects = new Models.Subject[6];

        Models.Subject[] Subjects
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

            //ComboBox_1.ItemsSource = items;
            //ComboBox_2.ItemsSource = items;
            //ComboBox_3.ItemsSource = items;
            //ComboBox_4.ItemsSource = items;
            //ComboBox_5.ItemsSource = items;
            //ComboBox_6.ItemsSource = items;
        }

        private void ComboBox_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string name = comboBox.Name;
            int boxNum = name[-1];
            Subjects[boxNum] = (Models.Subject)comboBox.SelectedIndex;
        }
    }
}
