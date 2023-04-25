using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ToolKit.Classes;

namespace Toolkit.Windows
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
        Timer timer = new();
        List<string> AutoScrollText = new();
        int count = 0;

        Queue<string> FormulaQueue = new(), TextQueue = new();

        public NotifyWindow()
        {
            InitializeComponent();

            Task.Run(SetFormula);
            Task.Run(SetText);
            Task.Run(Mod);

            EnqueueText("事件：启动");
            timer.Interval = 3000; //1800000;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Mod()
        {
            bool TimeShown = false;
            //Dictionary<DateTime, bool> Times = new();
            while (true)
            {
                Settings CurrentSettings = App.AppSettings;
                if (CurrentSettings.Mod_Time && DateTime.Now.Minute == 0 && !TimeShown)
                {
                    EnqueueText($"现在时间：{DateTime.Now.Hour}:{00}");
                    TimeShown = true;
                }
                else TimeShown = false;

                //if (CurrentSettings.Mod_Weather.AutoWeather)
                //{
                //    foreach (var time in CurrentSettings.Mod_Weather.Times)
                //    {
                //        if (DateTime.Now.Hour == time.Hour 
                //            && DateTime.Now.Minute == time.Minute
                //            && !Times.ContainsKey(time))
                //        {
                //            var life = PuranLai.APIs.WebApi.GetWeatherInformation(
                //                PuranLai.APIs.WebApi.GetIpInformation(PuranLai.APIs.WebApi.GetHostIp()).Adcode).Lives[0];
                //            EnqueueText($"当前天气：{life.Weather}，{life.Temperature}°");
                //            CurrentSettings.Mod_Weather.Times.Remove(time);
                //        }
                //    }
                //}


            }
        }

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
                    System.Windows.Media.Brushes.Black,
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
                    Random rand = new();
                    showText = AutoScrollText[rand.Next(0, AutoScrollText.Count - 1)];
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

            FormulaQueue.Enqueue(formula);
        }

        private async void SetFormula()
        {
            while (true)
            {
                if (FormulaQueue.Count > 0)
                {
                    string formula = FormulaQueue.Dequeue();

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
        }

        public void EnqueueText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            TextQueue.Enqueue(text);
        }

        private async void SetText()
        {
            while (true)
            {
                if (App.TaskbarIconToolTip is not null)
                {
                    App.TaskbarIconToolTip.Dispatcher.Invoke(() =>
                    {
                        App.TaskbarIconToolTip.Content = "PuranLai's ToolKit\n队列中字条数量：" + TextQueue.Count;
                    });
                }

                if (TextQueue.Count > 0)
                {
                    string text = TextQueue.Dequeue();

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
                }
            }
        }

        private async void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            await Task.Delay(10000);
            Visibility = Visibility.Visible;
        }
    }
}
