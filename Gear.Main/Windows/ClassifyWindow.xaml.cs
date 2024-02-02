using Gear.Windows;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Gear.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gear.Windows
{
    /// <summary>
    /// ClassifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClassifyWindow : Window
    {
        DateTime LastOpen;
        bool DoubleClicked = false;
        public ClassifyWindow()
        {
            InitializeComponent();
            Operations.ToBottom(this);
            Left = (SystemParameters.PrimaryScreenWidth - Width) * 0.5;
            Top = SystemParameters.PrimaryScreenHeight * 0.1;

            CheckSubjectFolders();

            for (int i = 0; i < 6; i++)
            {
                SetButtonSubject(App.AppSettings.Subjects[i], i + 1);
            }
        }

        public class SubjectDetail
        {
            public SubjectDetail(Button button)
            {
                var subject = Models.Subject.GetSubjects((string)button.Tag);
                if (subject is not null)
                    Subject = (Subject.SchoolSubject)subject;

                ResourceDictionary dictionary = new()
                {
                    Source = new(@"\Resources\SubjectTranslations\zh-cn.xaml", UriKind.Relative)
                };
                SubjectName = (string)dictionary[Subject.ToString()];
                Directory = $@"{App.AppSettings.RootDirectory}\{SubjectName}\";
            }

            public SubjectDetail(Subject.SchoolSubject subject)
            {
                Subject = subject;
                ResourceDictionary dictionary = new()
                {
                    Source = new(@"\Resources\SubjectTranslations\zh-cn.xaml", UriKind.Relative)
                };
                SubjectName = (string)dictionary[subject.ToString()];
                Directory = $@"{App.AppSettings.RootDirectory}\{SubjectName}\";
            }
            public Subject.SchoolSubject Subject { get; set; } = Models.Subject.SchoolSubject.Chinese;
            public string SubjectName { get; set; }
            public string Directory { get; set; }
        }

        private static void CheckSubjectFolders()
        {
            List<string> directories = new();
            foreach (var subject in App.AppSettings.Subjects)
            {
                var detail = new SubjectDetail(subject);
                if (!Directory.Exists(detail.Directory))
                {
                    directories.Add(detail.Directory);
                }
            }

            if (directories.Count != 0)
            {
                MessageWindow window = new("确定创建所有缺失的文件夹？", Convert.ToChar(0xE1DA).ToString());
                window.ShowDialog();
                if (window.Result)
                {
                    foreach (var directory in directories)
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
            }
        }

        public void SetButtonSubject(Subject.SchoolSubject subject, int num)
        {
            try
            {
                var subjectDetail = new SubjectDetail(subject);
                var button = (Button)FindName($"Button_{num}");
                button.Tag = subject.ToString();
                var textBlock = (TextBlock)FindName($"TextBlock_{num}");
                textBlock.Text = subjectDetail.SubjectName;
            }
            catch (Exception ex)
            {
                new MessageWindow($"{ex.TargetSite}\n{ex.InnerException}\n{ex.Message}").Show();
            }
        }

        private static void SelectFiles(SubjectDetail detail)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "选择课件",
                Filter = "课件|*.*",
                FileName = string.Empty,
                FilterIndex = 1,
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (Directory.Exists("E:\\"))
            {
                openFileDialog.InitialDirectory = "E:\\";
            }
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            string[] files = openFileDialog.FileNames;
            CopyFiles(detail, files);
        }

        private static void GetDroppedFiles(SubjectDetail detail, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Contains("Close"))
                {
                    Application.Current.Shutdown();
                    return;
                }
                CopyFiles(detail, files);
            }
            catch { }
        }

        private static void CopyFiles(SubjectDetail detail, string[] files)
        {
            string folder = detail.Directory;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            foreach (string file in files)
            {
                string destination = folder + Path.GetFileName(file);
                if (destination == file)
                {
                    continue;
                }
                else if (File.Exists(destination))
                {
                    FileSystem.DeleteFile(destination, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                File.Copy(file, destination);

                DirectoryInfo? info = Directory.GetParent(file);
                if (info is null)
                    return;
                if (info.FullName == Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
                {
                    File.Delete(file);
                }
            }
        }

        private void DropButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var detail = new SubjectDetail((Button)sender);
            SelectFiles(detail);
        }

        private async void DropButton_Drop(object sender, DragEventArgs e)
        {
            var detail = new SubjectDetail((Button)sender);
            App.Notifier.EnqueueText("事件：移动文件到" + detail.SubjectName + "文件夹");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                GetDroppedFiles(detail, e);
            }
            await Task.Delay(1000);
            App.Notifier.EnqueueText("事件：移动完成");

        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Operations.ToBottom(this);
        }

        private async void DropButton_Click(object sender, RoutedEventArgs e)
        {
            WaitProgressRing.IsActive = true;
            WaitProgressRing.Visibility = Visibility.Visible;
            await Task.Delay(500);
            if (DoubleClicked)
            {
                DoubleClicked = false;
                return;
            }

            var detail = new SubjectDetail((Button)sender);
            WaitProgressRing.IsActive = false;
            WaitProgressRing.Visibility = Visibility.Collapsed;
            if (DateTime.Now - LastOpen <= TimeSpan.FromMilliseconds(1500))
            {
                return;
            }
            Process.Start("explorer.exe", detail.Directory);
            LastOpen = DateTime.Now;

            App.Notifier.EnqueueText("事件：打开" + detail.SubjectName + "文件夹");
        }

        private void DropButton_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Notifier.EnqueueText("事件：选择文件");
            DoubleClicked = true;
            WaitProgressRing.IsActive = false;
            WaitProgressRing.Visibility = Visibility.Collapsed;
            SelectFiles(new SubjectDetail((Button)sender));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

        }

        private void Note_Click(object sender, RoutedEventArgs e)
        {
            App.Notifier.EnqueueText("事件：打开笔记文件夹");
            Process.Start("explorer.exe",
                Environment.GetFolderPath
                (Environment.SpecialFolder.MyPictures) + "\\Ink Canvas Screenshots");
        }
    }
}
