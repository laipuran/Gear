using Gear.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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
            DirectoryTextBox.Text = App.AppSettings.RootDirectory;
            Subjects = App.AppSettings.Subjects;

            InitializeComboBoxes();
        }

        private void InitializeComboBoxes()
        {
            var items = new List<string>
            {
                "语文","数学","英语","物理",
                "化学","生物","政治","历史",
                "地理",
            };

            var boxes = new List<ComboBox>
            {
                ComboBox_1, ComboBox_2, ComboBox_3,
                ComboBox_4, ComboBox_5, ComboBox_6
            };

            foreach (var box in boxes)
            {
                box.ItemsSource = items;
                box.SelectedIndex = (int)App.AppSettings.Subjects[boxes.IndexOf(box)];
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string name = comboBox.Name;
            int index = PuranLai.Algorithms.Parse.ParseFromString(name[name.Length - 1].ToString(), 6).number;
            var schoolSubject = (Models.Subject.SchoolSubject)comboBox.SelectedIndex;
            Subjects[index - 1] = schoolSubject;
            App.Classifier.SetButtonSubject(schoolSubject, index);
        }

        #region Quick Settings
        private void Button_PCB_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox_4.SelectedIndex = 3;
            ComboBox_5.SelectedIndex = 4;
            ComboBox_6.SelectedIndex = 5;
        }

        private void Button_PCG_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox_4.SelectedIndex = 3;
            ComboBox_5.SelectedIndex = 4;
            ComboBox_6.SelectedIndex = 8;
        }

        private void Button_HPG_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox_4.SelectedIndex = 6;
            ComboBox_5.SelectedIndex = 7;
            ComboBox_6.SelectedIndex = 8;
        }
        #endregion

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(DirectoryTextBox.Text);
            }
            catch (Exception ex)
            {
                new MessageWindow($"文件夹路径不合法\n{ex.Message}\n{ex.InnerException}").Show();
                return;
            }
            App.AppSettings.RootDirectory = DirectoryTextBox.Text;
        }
    }
}
