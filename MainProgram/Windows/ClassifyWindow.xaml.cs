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
            Left = (SystemParameters.PrimaryScreenWidth - Width) * 0.5;
            Top = SystemParameters.PrimaryScreenHeight * 0.05;
        }

        private static void SelectFiles(string subject)
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
            CopyFiles(subject, files);
        }

        private static void GetDroppedFiles(string subject, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Contains("Close"))
                {
                    Application.Current.Shutdown();
                    return;
                }
                CopyFiles(subject, files);
            }
            catch { }
        }

        private static void CopyFiles(string subject, string[] files)
        {
            string folder = $"D:/{subject}/";
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
                if (Directory.GetParent(file).FullName ==
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
                {
                    File.Delete(file);
                }
            }
        }

        private void DropButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectFiles(((Button)sender).Content.ToString());
        }

        private async void DropButton_Drop(object sender, DragEventArgs e)
        {
            ((Button)sender).Dispatcher.Invoke(() =>
            {
                App.Notifier.EnqueueText("事件：移动文件到" + ((Button)sender).Content.ToString() + "文件夹");
            });
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                GetDroppedFiles(((Button)sender).Content.ToString(), e);
            }
            await Task.Delay(1000);
            App.Notifier.EnqueueText("事件：移动完成");

        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Operations.ToBottom(this);
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

            WaitProgressRing.IsActive = false;
            WaitProgressRing.Visibility = Visibility.Collapsed;
            if (DateTime.Now - LastOpen <= TimeSpan.FromMilliseconds(1500))
            {
                return;
            }
            Process.Start("explorer.exe", $"D:\\{((Button)sender).Content}");
            LastOpen = DateTime.Now;

            ((Button)sender).Dispatcher.Invoke(() =>
            {
                App.Notifier.EnqueueText("事件：打开" + ((Button)sender).Content.ToString() + "文件夹");
            });
        }

        private void DropButton_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() =>
            {
                App.Notifier.EnqueueText("事件：选择文件");
            });
            DoubleClicked = true;
            WaitProgressRing.IsActive = false;
            WaitProgressRing.Visibility = Visibility.Collapsed;
            SelectFiles(((Button)sender).Content.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            App.Notifier.EnqueueText("想关掉我？没门");
        }

        private void Note_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                App.Notifier.EnqueueText("事件：打开笔记文件夹");
            });
            Process.Start("explorer.exe",
                Environment.GetFolderPath
                (Environment.SpecialFolder.MyPictures) + "\\Ink Canvas Screenshots");
        }
    }
}
