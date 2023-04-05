using CommunityToolkit.Mvvm.Input;
using Hardcodet.Wpf.TaskbarNotification;
using ModernWpf.Controls;
using System.Windows;
using System.Windows.Controls;
using Toolkit.Windows;

namespace Toolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ClassifyWindow Classifier = new() { Visibility = Visibility.Visible };
        public static NotifyWindow Notifier = new();
        public static TaskbarIcon TaskbarIcon { get; private set; }
        public static ContextMenu TaskbarIconContextMenu { get; private set; }
        public static ToolTip TaskbarIconToolTip { get; private set; }
        public static MenuItem SettingsItem { get; private set; }
        public static bool ShowTimer = false;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetupTrayIcon();
            Notifier.Show();
            Classifier.Show();
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
                IsOn = ShowTimer,
            };
            timerVisibilityOption.Toggled += TimerVisibilityToggleSwitch_Toggled;

            SettingsItem.Items.Add(classifierVisibilityOption);
            SettingsItem.Items.Add(timerVisibilityOption);

            var showMainWindowItem = new MenuItem()
            {
                Header = "显示主窗口",
                ToolTip = "将主窗口透明的改为1",
                Icon = new FontIcon() { Glyph = "\uE70A" },
                Command = new RelayCommand(() => { MainWindow.Opacity = 1; })
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
                Items = { SettingsItem, /*showMainWindowItem,*/ exitItem }
            };

            TaskbarIconToolTip = new()
            {
                Content = "PuranLai's ToolKit"
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
                Notifier.SetText("事件：修改课件分类[可见性]为[可见]");
            }
            else
            {
                Classifier.Visibility = Visibility.Collapsed;
                Notifier.SetText("事件：修改课件分类[可见性]为[不可见]");

            }
        }

        private void TimerVisibilityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ShowTimer = !ShowTimer;
            string flag = ShowTimer ? "是" : "否";
            Notifier.SetText($"事件：修改计时器事件[活动性]为[{flag}]");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TaskbarIcon.Visibility = Visibility.Collapsed;
        }
    }
}
