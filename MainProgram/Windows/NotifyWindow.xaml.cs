using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Toolkit.Windows
{
    public enum LoopMode
    {
        Shuffle,
        Normal
    }

    /// <summary>
    /// NotifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyWindow : Window
    {
        Storyboard BoardOpen = new(), BoardClose = new();
        DoubleAnimation Open = new(), Close = new();
        Timer timer = new();
        List<string> AutoScrollText = new();
        int count = 0;
        public NotifyWindow()
        {
            InitializeComponent();
            SetupAnimations();
            SetText("事件：启动");
            timer.Interval = 3000;//1800000;
            timer.Elapsed += Timer_Elapsed;
        }

        private void SetupAnimations()
        {
            Open = new()
            {
                From = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };

            ElasticEase ElasticEase = new()
            {
                Oscillations = 2,
                Springiness = 5
            };
            Open.EasingFunction = ElasticEase;
            Storyboard.SetTargetName(Open, "ContentTextBlock");
            Storyboard.SetTargetProperty(Open, new(WidthProperty));
            BoardOpen.Children.Add(Open);

            Close = new()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };

            BackEase BackEase = new() { EasingMode = EasingMode.EaseIn };
            Close.EasingFunction = BackEase;
            BoardClose.Children.Add(Close);

            Storyboard.SetTargetName(Close, "ContentTextBlock");
            Storyboard.SetTargetProperty(Close, new(WidthProperty));
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            string showText = "";
            if (App.ShowAutoScroll)
            {
                if (App.Loop == LoopMode.Normal)
                {
                    if (count >= AutoScrollText.Count)
                    {
                        count = 0;
                    }
                    showText = AutoScrollText[count];
                    count++;
                }
                else if (App.Loop == LoopMode.Shuffle)
                {
                    Random rand = new();
                    showText = AutoScrollText[rand.Next(0, AutoScrollText.Count - 1)];
                }
                timer.Interval = showText.Length * 300 + 10000;
                SetText(showText);
            }
            timer.Start();
        }

        public void SetScroller(List<string> strings)
        {
            timer.Stop();

            AutoScrollText = strings;

            if (App.Loop == LoopMode.Normal)
            {
                count = 0;
            }
            timer.Interval = 1000;
            timer.Start();
        }

        public async void SetText(string? text)
        {
            try
            {
                BoardOpen.Stop(ContentTextBlock);
                BoardOpen.Stop(ContentTextBlock);
            }
            catch { }
            if (string.IsNullOrEmpty(text)) return;
            
            await ContentTextBlock.Dispatcher.Invoke(async () =>
            {
                FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(ContentTextBlock.FontFamily, ContentTextBlock.FontStyle, ContentTextBlock.FontWeight, ContentTextBlock.FontStretch),
                ContentTextBlock.FontSize,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

                ContentTextBlock.Text = text;
                Open.To = formattedText.Width;
                BoardOpen.Begin(ContentTextBlock);

                int delay = 0;
                if (text.Length >= 10)
                {
                    delay = text.Length * 100 + 5000;
                }
                await Task.Delay(delay);

                Close.From = formattedText.Width;
                BoardClose.Begin(ContentTextBlock);
                await Task.Delay(2000);
                ContentTextBlock.Text = "";
            });
        }

    }
}
