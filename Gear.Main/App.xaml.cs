using CommunityToolkit.Mvvm.Input;
using Gear.Base.Interface;
using Gear.Models;
using Gear.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using iNKORE.UI.WPF.Modern.Controls;
using IWshRuntimeLibrary;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using File = System.IO.File;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

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
        public static WebApplication WebApp = RestApi.Program.CreateWebApp([]);
        public static INotifyQueueService NotificationQueueService =
            RestApi.Controllers.TextNotification.NotifyService;
#pragma warning restore CA2211 // 非常量字段应当不可见

        #region Initialize Taskbar Icon Components
        public static TaskbarIcon? TaskbarIcon { get; private set; }
        public static ContextMenu TaskbarIconContextMenu { get; private set; } = new();
        public static ToolTip TaskbarIconToolTip { get; private set; } = new();
        public static MenuItem SettingsItem { get; private set; } = new();
        #endregion


        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length != 0)
            {
                var args = e.Args.ToList();
                foreach (var arg in args)
                {
                    if (arg == "/notify" || arg=="-notify")
                    {
                        new NotifyWindow(args[args.IndexOf(arg) + 1]).Show();
                        await Task.Delay(1800);
                        break;
                    }
                }
                Console.WriteLine(string.Join('|', args));
                Environment.Exit(0);
            }

            ForerunCheck();

            //new NotifyWindowExtended("Test 测试").Show();
            //return;

            WebApp.RunAsync("http://localhost:5177");
#if DEBUG
            //Process.Start("explorer", "http://localhost:5177/swagger");
#endif

            Notifier.Show();

            if (App.AppSettings.EnableCountdown && App.AppSettings.CountdownDate is not null)
            {
                App.Notifier.ShowToast($"离 {App.AppSettings.CountDownEventName} 还有 {(App.AppSettings.CountdownDate - DateTime.Now).Value.Days} 天！");
            }
            else
                Notifier.ShowToast("事件：启动");

            Classifier.Show();
            SetupTrayIcon();
            SetupAutoStart();
        }

        private static void ForerunCheck()
        {
            Process[] process = Process.GetProcessesByName("Gear.Desktop");
            if (process.Length > 1)
            {
                MessageBox.Show("三叉戟 存在运行中的实例！", "三叉戟");
                Environment.Exit(-1);
            }
        }

        private void SetupAutoStart()
        {
            const string shortcutName = "Gear.lnk";
            string startupMenu = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            List<string> files = [.. Directory.GetFiles(startupMenu)];
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
            shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, "Gear.Desktop.exe");
            shortcut.WorkingDirectory = Path.Combine(Environment.CurrentDirectory);
            shortcut.IconLocation = Path.Combine(Environment.CurrentDirectory, "Icon.ico");
            shortcut.Save();
        }

        private void SetupTrayIcon()
        {

            SettingsItem = new MenuItem()
            {
                Header = "快捷设置",
                ToolTip = "对程序进行快速设置",
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
            autoStartOption.MouseRightButtonDown += AutoStartOption_MouseRightButtonDown;

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
                Content = "Gear"
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

        /// <summary>
        /// 打开 开机启动 文件夹
        /// </summary>
        private void AutoStartOption_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
        }

        private void AutoStartOption_Toggled(object sender, RoutedEventArgs e)
        {
            AppSettings.AutoStart = !AppSettings.AutoStart;
            SetupAutoStart();
        }

        private void ClassifierVisibilityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //AppSettings.ShowClassifier = ((ToggleSwitch)sender).IsOn;
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
