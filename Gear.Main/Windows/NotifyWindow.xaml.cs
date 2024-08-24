using Gear.Base.Class;
using Gear.Models;
using iNKORE.UI.WPF.Modern.Media.Animation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Gear.Windows
{
    public enum LoopMode
    {
        Shuffle,
        Normal
    }

    public enum DisplayMode
    {
        Text,
        Formula
    }
    /// <summary>
    /// NotifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyWindow : Window
    {
        DisplayMode Mode = DisplayMode.Text;
        List<string> AutoScrollText = new();
        int count = 0; Timer timer = new();
        Task Task_Mod, Task_Formula, Task_Text;
        string Message = "";

        public NotifyWindow()
        {
            InitializeComponent();

            MainBorder.Opacity = 0;
#if DEBUG
            DebugButton.Visibility = Visibility.Visible;
#endif
            #region Set Tasks
            Task_Mod = new(Mod);
            Task_Formula = new(SetFormula);
            Task_Text = new(SetText);

            Task_Mod.Start();
            Task_Formula.Start();
            Task_Text.Start();

            timer.Interval = 3000;
            timer.Elapsed += Timer_Elapsed;
            #endregion
        }

        private async void Mod()
        {
            while (true)
            {
                Settings settings = App.AppSettings;
                TimeOnly time = TimeOnly.FromDateTime(DateTime.Now);
                if (time.Second != 0)
                    continue;

                foreach (var timer in settings.Mod_Timing)
                {
                    if (Equals(time, timer.Key))
                    {
                        EnqueueText($"{timer.Key}定时事项：{timer.Value}");
                    }
                }

                await Task.Delay(1000);
            }
        }

        private static bool Equals(TimeOnly time1, TimeOnly time2)
        {
            return time1.Hour == time2.Hour &&
                time1.Minute == time2.Minute;
        }

        /// <summary>
        /// 获取关键的 Storyboard
        /// </summary>
        /// <param name="mode">展示的模式</param>
        /// <param name="text">显示的内容</param>
        /// <returns></returns>
        private List<Storyboard> GetAnimation(DisplayMode mode, string text)
        {
            DoubleAnimation open = new()
            {
                From = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3)),

                EasingFunction = new ElasticEase()
                {
                    Oscillations = 1,
                    Springiness = 3
                }
            };

            DoubleAnimation close = new()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(1.5)),

                EasingFunction = new BackEase()
                {
                    EasingMode = EasingMode.EaseIn,
                    Amplitude = 0.5
                }
            };

            if (mode == DisplayMode.Text)
            {
                Storyboard.SetTargetProperty(open, new(WidthProperty));
                Storyboard.SetTargetProperty(close, new(WidthProperty));

                FormattedText formattedText = new(
                    text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new(ContentTextBlock.FontFamily, ContentTextBlock.FontStyle, ContentTextBlock.FontWeight, ContentTextBlock.FontStretch),
                    ContentTextBlock.FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                double value = formattedText.Width;

                open.To = value;
                close.From = value;
            }
            else if (mode == DisplayMode.Formula)
            {
                Storyboard.SetTargetProperty(open, new(HeightProperty));
                Storyboard.SetTargetProperty(close, new(HeightProperty));

                open.To = 60;
                close.From = 60;
            }

            Storyboard Open = new(); Open.Children.Add(open);
            Storyboard Close = new(); Close.Children.Add(close);

            return new() { Open, Close };
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            string showText = "";
            if (App.AppSettings.AutoScroll)
            {
                if (App.AppSettings.RollerText.LoopMode == LoopMode.Normal)
                {
                    if (count >= AutoScrollText.Count)
                    {
                        count = 0;
                    }
                    showText = AutoScrollText[count];
                    count++;
                }
                else if (App.AppSettings.RollerText.LoopMode == LoopMode.Shuffle)
                {
                    showText = AutoScrollText[Random.Shared.Next(0, AutoScrollText.Count - 1)];
                }
                timer.Interval = showText.Length * 100 + 15000;

                if (Mode == DisplayMode.Text)
                {
                    EnqueueText(showText);
                }
                else if (Mode == DisplayMode.Formula)
                {
                    EnqueueFormula(showText);
                }
            }
            timer.Start();
        }

        public void SetScroller(List<string> strings, DisplayMode mode)
        {
            timer.Stop();

            Mode = mode;
            AutoScrollText = strings;

            if (App.AppSettings.RollerText.LoopMode == LoopMode.Normal)
            {
                count = 0;
            }
            timer.Interval = 100;
            timer.Start();
        }

        private void EnqueueFormula(string formula)
        {
            if (string.IsNullOrEmpty(formula)) return;

            App.NotificationQueueService.EnqueueNotification(new(ContentForm.Formula, formula));
        }

        private async void SetFormula()
        {
            while (true)
            {
                await Task.Delay(1500);
                if (App.NotificationQueueService.Count(ContentForm.Formula) <= 0)
                {
                    continue;
                }

                var @object = App.NotificationQueueService.DequeueNotification(ContentForm.Formula);
                if (@object is null)
                    continue;
                string formula = @object.Content;

                await ContentFormulaControl.Dispatcher.Invoke(async () =>
                {
                    var boards = GetAnimation(DisplayMode.Formula, formula);
                    Storyboard BoardOpen = boards[0];
                    ContentFormulaControl.Formula = formula;
                    BoardOpen.Begin(FormulaViewBox);

                    int delay = 5000;
                    await Task.Delay(delay);

                    Storyboard BoardClose = boards[1];
                    BoardClose.Begin(FormulaViewBox);
                    await Task.Delay(2000);

                    FormulaViewBox.Height = 0;
                });
            }
        }

        public void EnqueueText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            App.NotificationQueueService.EnqueueNotification(new(Base.Class.ContentForm.Text, text));
        }

        private async void SetText()
        {
            while (true)
            {
                await Task.Delay(1000);
                if (App.TaskbarIconToolTip is not null)
                {
                    try
                    {
                        App.TaskbarIconToolTip.Dispatcher.Invoke(() =>
                        {
                            App.TaskbarIconToolTip.Content = "Gear\n队列中字条数量：" + App.NotificationQueueService.Count(Base.Class.ContentForm.Text);
                        });
                    }
                    catch { }
                }

                if (App.NotificationQueueService.Count(Base.Class.ContentForm.Text) <= 0)
                {
                    continue;
                }

                var @object = App.NotificationQueueService.DequeueNotification(Base.Class.ContentForm.Text);
                if (@object is null)
                    continue;
                string text = @object.Content;
                
                await Dispatcher.BeginInvoke(() => { BeginAnimation(); ContentTextBlock.Text = text; });

                try
                {
                    /*
                    await ContentTextBlock.Dispatcher.Invoke(async () =>
                    {
                        var boards = GetAnimation(DisplayMode.Text, text);
                        Storyboard BoardOpen = boards[0];
                        ContentTextBlock.Text = text;
                        BoardOpen.Begin(ContentTextBlock);

                        int delay = 3000;
                        if (text.Length > 10)
                        {
                            delay = text.Length * 100 + 5000;
                        }
                        await Task.Delay(delay);

                        Storyboard BoardClose = boards[1];
                        BoardClose.Begin(ContentTextBlock);
                        await Task.Delay(2000);

                        ContentTextBlock.Width = 0;    
                    });
                    */
                }
                catch { }
            }
        }

        private async void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            await Task.Delay(10000);
            Visibility = Visibility.Visible;
        }

        public void ClearTexts()
        {
            int count = App.NotificationQueueService.Count(Base.Class.ContentForm.Text);
            for (int i = 1; i <= count; i++)
            {
                App.NotificationQueueService.DequeueNotification(Base.Class.ContentForm.Text);
            }
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            BeginAnimation();
        }

        private async void BeginAnimation()
        {
            #region Initialize Animations
            Storyboard Open_Border = new(), Open_Lorem = new(),
                Close_Border = new(), Close_Lorem = new();

            DoubleAnimation Open_Height = new()
            {
                From = SystemParameters.PrimaryScreenHeight,
                To = 300,
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new BackEase() { Amplitude = 0.3},
            };

            DoubleAnimation Open_Opacity = new()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new CubicEase(),
            };

            ThicknessAnimation Open_Margin = new()
            {
                From = new(2000, 0, 0, 0),
                To = new(0, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new BackEase()
                {
                    Amplitude = 0.7,
                    EasingMode = EasingMode.EaseOut
                }
            };
            Storyboard.SetTargetProperty(Open_Height, new(HeightProperty));
            Storyboard.SetTargetProperty(Open_Opacity, new(OpacityProperty));
            Storyboard.SetTargetProperty(Open_Margin, new(MarginProperty));

            Open_Border.Children.Add(Open_Height);
            Open_Lorem.Children.Add(Open_Margin);
            Open_Border.Children.Add(Open_Opacity);

            DoubleAnimation Close_Height = new()
            {
                From = 300,
                To = SystemParameters.PrimaryScreenHeight,

                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new BackEase()
                {
                    Amplitude = 0.5,
                    EasingMode = EasingMode.EaseIn
                }
            };

            DoubleAnimation Close_Opacity = new()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new CubicBezierEase()
                {
                    EasingMode = EasingMode.EaseIn
                }
            };

            ThicknessAnimation Close_Margin = new()
            {
                From = new(0, 0, 0, 0),
                To = new(2000, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new BackEase()
                {
                    Amplitude = 0.6,
                    EasingMode = EasingMode.EaseIn
                }
            };
            Storyboard.SetTargetProperty(Close_Height, new(HeightProperty));
            Storyboard.SetTargetProperty(Close_Opacity, new(OpacityProperty));
            Storyboard.SetTargetProperty(Close_Margin, new(MarginProperty));

            Close_Border.Children.Add(Close_Height);
            Close_Border.Children.Add(Close_Opacity);
            Close_Lorem.Children.Add(Close_Margin);
            #endregion

            Open_Border.Begin(MainBorder);
            Open_Lorem.Begin(ContentTextBlock);
            await Task.Delay(1200);
            Close_Border.Begin(MainBorder);
            Close_Lorem.Begin(ContentTextBlock);

            //Close();
        }
    }
}
