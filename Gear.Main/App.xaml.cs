using CommunityToolkit.Mvvm.Input;
using Gear.Base.Interface;
using Gear.Models;
using Gear.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using IWshRuntimeLibrary;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using File = System.IO.File;

namespace Gear
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#pragma warning disable CA2211 // 非常量字段应当不可见
        public static Settings AppSettings = Settings.GetSettings();
        public static ClassifyWindow Classifier = new();
        public static NotifyWindow Notifier = new();
        public static WebApplication WebApp = RestApi.Program.CreateWebApp(new string[0]);
        public static INotifyQueueService NotificationQueueService =
            RestApi.Controllers.TextNotification.NotifyService;
#pragma warning restore CA2211 // 非常量字段应当不可见

        #region Initialize Taskbar Icon Components
        public static TaskbarIcon? TaskbarIcon { get; private set; }
        public static ContextMenu TaskbarIconContextMenu { get; private set; } = new();
        public static ToolTip TaskbarIconToolTip { get; private set; } = new();
        public static MenuItem SettingsItem { get; private set; } = new();
        #endregion

        #region Include System Functions
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string? lpClassName,
                                                string? lpWindowName);
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length != 0)
            {
                var args = e.Args.ToList();
                new MessageWindow(args[0]).ShowDialog();
                Console.WriteLine(string.Join(' ', args));
                Environment.Exit(0);
            }

            // Check exist instance of this app
            ForeRunCheck();
            // Show the windows

            WebApp.RunAsync("http://localhost:5177");
            System.Diagnostics.Process.Start("explorer", "http://localhost:5177/swagger");

            Notifier.Show();
            Notifier.EnqueueText("事件：启动");
            Classifier.Show();
            // Setup the tray icon
            SetupTrayIcon();
            // Configure startup
            SetupAutoStart();
        }

        private static void ForeRunCheck()
        {
            IntPtr hWnd = FindWindow(null, "三叉戟：设置");                     //Avoiding opening this app twice
            if (hWnd != IntPtr.Zero)
            {
                MessageBox.Show("三叉戟 存在运行中的实例！", "三叉戟");
                Environment.Exit(-1);
            }
        }

        private void SetupAutoStart()
        {
            const string shortcutName = "ProngedGear.lnk";
            string startupMenu = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            List<string> files = Directory.GetFiles(startupMenu).ToList();
            string fullName = startupMenu + "\\" + shortcutName;
            if (files.Contains(fullName))
            {
                File.Delete(fullName);
            }

            if (!AppSettings.AutoStart)
            {
                return;
            }

            WshShell shell = new();
            IWshShortcut shortcut = shell.CreateShortcut(Path.Combine(startupMenu, shortcutName));
            shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, "Gear.exe");
            shortcut.WorkingDirectory = Path.Combine(Environment.CurrentDirectory);
            shortcut.IconLocation = Path.Combine(Environment.CurrentDirectory, "Icon.ico");
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
                IsOn = true,
            };
            classifierVisibilityOption.Toggled += ClassifierVisibilityToggleSwitch_Toggled;

            var timerVisibilityOption = new ToggleSwitch()
            {
                Header = "是否定时显示",
                IsOn = AppSettings.AutoScroll,
            };
            timerVisibilityOption.Toggled += TimerVisibilityToggleSwitch_Toggled;

            var autoStartOption = new ToggleSwitch()
            {
                Header = "是否开机自启动",
                IsOn = AppSettings.AutoStart
            };
            autoStartOption.Toggled += AutoStartOption_Toggled;

            SettingsItem.Items.Add(classifierVisibilityOption);
            SettingsItem.Items.Add(timerVisibilityOption);
            SettingsItem.Items.Add(autoStartOption);

            var showMainWindowItem = new MenuItem()
            {
                Header = "显示主窗口",
                ToolTip = "显示设置窗口",
                Icon = new FontIcon() { Glyph = "\uE70A" },
                Command = new RelayCommand(() =>
                {
                    try
                    {
                        MainWindow.Close();
                    }
                    catch { }
                    MainWindow = new MainWindow();
                    MainWindow.Show();
                })
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
                LeftClickCommand = new RelayCommand(() => { TaskbarIconContextMenu.IsOpen = !TaskbarIconContextMenu.IsOpen; }),
            };
        }

        private void AutoStartOption_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.AutoStart = !AppSettings.AutoStart;
            SetupAutoStart();
        }

        private void ClassifierVisibilityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
            {
                Classifier.Visibility = Visibility.Visible;
                //Notifier.EnqueueText("事件：修改课件分类[可见性]为[可见]");
            }
            else
            {
                Classifier.Visibility = Visibility.Collapsed;
                //Notifier.EnqueueText("事件：修改课件分类[可见性]为[不可见]");

            }
        }

        private void TimerVisibilityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.AutoScroll = !AppSettings.AutoScroll;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Save(AppSettings);
            if (TaskbarIcon is not null)
                TaskbarIcon.Visibility = Visibility.Collapsed;
        }
    }
}
