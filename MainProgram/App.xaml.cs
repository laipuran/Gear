using CommunityToolkit.Mvvm.Input;
using Hardcodet.Wpf.TaskbarNotification;
using IWshRuntimeLibrary;
using ModernWpf.Controls;
using ProngedGear.Models;
using ProngedGear.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using File = System.IO.File;

namespace ProngedGear
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Settings AppSettings = Settings.GetSettings();
        #region Initialize Taskbar Icon Components
        public static ClassifyWindow Classifier = new() { Visibility = Visibility.Visible };
        public static NotifyWindow Notifier = new();
        public static TaskbarIcon TaskbarIcon { get; private set; } = new();
        public static ContextMenu TaskbarIconContextMenu { get; private set; } = new();
        public static ToolTip TaskbarIconToolTip { get; private set; } = new();
        public static MenuItem SettingsItem { get; private set; } = new();
        #endregion
        #region Include System Functions
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string? lpClassName, string? lpWindowName);
        #endregion
        public static Settings AppSettings = new();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ForeRunCheck();
            SetupTrayIcon();
            SetupAutoStart();
            AppSettings = Settings.GetSettings();
            Notifier.Show();
            Classifier.Show();
        }

        private void ForeRunCheck()
        {
            IntPtr hWnd = FindWindow(null, "Over Top");                     //Avoiding opening this app twice
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("Over Top 存在运行中的实例！", "Over Top");
                Environment.Exit(-1);
            }
        }

        private void SetupAutoStart()
        {
            const string shortcutName = "ProngedGear.lnk";
            string StartUp = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            List<string> files = Directory.GetFiles(StartUp).ToList();
            if (files.Contains(shortcutName))
            {
                File.Delete(shortcutName);
            }

            WshShell shell = new();
            IWshShortcut shortcut = shell.CreateShortcut(Path.Combine(StartUp, shortcutName));
            shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, "Gear.exe");
            shortcut.WorkingDirectory = Path.Combine(Environment.CurrentDirectory);
            shortcut.IconLocation = Path.Combine(Environment.CurrentDirectory, "Icon.ico");
            shortcut.WindowStyle = 1;

            shortcut.Save();
        }

        private void SetupTrayIcon()
        {
            SettingsItem = new MenuItem()
            {
                Header = "设置",
                ToolTip = "对程序进行设置",
                Icon = new FontIcon() { Glyph = "\uE713" },
                Command = new RelayCommand(() => { SettingsItem.ContextMenu.IsOpen = !SettingsItem.ContextMenu.IsOpen; })
            };

            var classifierVisibilityOption = new ToggleSwitch()
            {
                Header = "是否显示课件分类",
                IsOn = Classifier.Visibility == Visibility.Visible,
            };
            classifierVisibilityOption.Toggled += ClassifierVisibilityToggleSwitch_Toggled;

            var timerVisibilityOption = new ToggleSwitch()
            {
                Header = "是否定时显示",
                IsOn = AppSettings.AutoScroll,
            };
            timerVisibilityOption.Toggled += TimerVisibilityToggleSwitch_Toggled;

            SettingsItem.Items.Add(classifierVisibilityOption);
            SettingsItem.Items.Add(timerVisibilityOption);

            var showMainWindowItem = new MenuItem()
            {
                Header = "显示主窗口",
                ToolTip = "显示设置窗口",
                Icon = new FontIcon() { Glyph = "\uE70A" },
                Command = new RelayCommand(() => { MainWindow.WindowState = WindowState.Normal; })
            };

            var exitItem = new MenuItem()
            {
                Header = "退出程序",
                ToolTip = "彻底关闭这个程序",
                Icon = new FontIcon() { Glyph = "\uE7E8" },
                Command = new RelayCommand(Current.Shutdown)
            };

            TaskbarIconContextMenu = new()
            {
                Items = { SettingsItem, showMainWindowItem, exitItem }
            };

            TaskbarIconToolTip = new()
            {
                Content = "Pronged Gear"
            };

            TaskbarIcon = new()
            {
                TrayToolTip = TaskbarIconToolTip,
                ContextMenu = TaskbarIconContextMenu,
                Icon = new System.Drawing.Icon("Icon.ico"),
                NoLeftClickDelay = true,
                LeftClickCommand = new RelayCommand(() => { TaskbarIconContextMenu.IsOpen = !TaskbarIconContextMenu.IsOpen; })
            };
        }

        private void ClassifierVisibilityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
            {
                Classifier.Visibility = Visibility.Visible;
                Notifier.EnqueueText("事件：修改课件分类[可见性]为[可见]");
            }
            else
            {
                Classifier.Visibility = Visibility.Collapsed;
                Notifier.EnqueueText("事件：修改课件分类[可见性]为[不可见]");

            }
        }

        private void TimerVisibilityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.AutoScroll = !AppSettings.AutoScroll;
            string flag = AppSettings.AutoScroll ? "是" : "否";
            Notifier.EnqueueText($"事件：修改计时器事件[活动性]为[{flag}]");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppSettings.Save();
            TaskbarIcon.Visibility = Visibility.Collapsed;
        }
    }
}
