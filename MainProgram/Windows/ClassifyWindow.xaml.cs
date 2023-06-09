using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using ProngedGear.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProngedGear.Windows
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
            Top = SystemParameters.PrimaryScreenHeight * 0.05;
        }

        public class SubjectDetail
        {
            public SubjectDetail(Button button)
            {
                var subject = Models.Subject.GetSubjects(button.Name);
                if (subject is not null)
                    Subject = (Subject.SchoolSubject)subject;

                ResourceDictionary dictionary = new()
                {
                    Source = new(@"\Resources\SubjectTranslations\zh-cn.xaml", UriKind.Relative)
                };
                SubjectName = (string)dictionary[Subject.ToString()];
                SubjectDirectory = $"D:/{SubjectName}/";
            }
            public Subject.SchoolSubject Subject { get; set; } = Models.Subject.SchoolSubject.Chinese;
            public string SubjectName { get; set; }
            public string SubjectDirectory { get; set; }
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
            string folder = detail.SubjectDirectory;
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
            //Operations.ToBottom(this);
            // NotifyWindow.SetText("事件：鼠标进入");
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
            Process.Start("explorer.exe", detail.SubjectDirectory);
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
            App.Notifier.EnqueueText("想关掉我？没门");
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
