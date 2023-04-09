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
        LoopMode Loop = LoopMode.Normal;
        int count = 0;
        public NotifyWindow()
        {
            InitializeComponent();
            SetText("事件：启动");
            NameScope.SetNameScope(this, new NameScope());
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
            Storyboard.SetTargetName(Open, ContentTextBlock.Name);
            Storyboard.SetTargetProperty(Open, new(WidthProperty));
            BoardOpen.Children.Add(Open);

            Close = new()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };

            BackEase BackEase = new() { EasingMode = EasingMode.EaseIn };
            Close.EasingFunction = BackEase;
            BoardClose.Children.Add(Close);

            Storyboard.SetTargetName(Close, ContentTextBlock.Name);
            Storyboard.SetTargetProperty(Close, new(WidthProperty));

            timer.Interval = 3000;//1800000;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (App.ShowAutoScroll)
            {
                if (Loop == LoopMode.Normal)
                {
                    if (count < AutoScrollText.Count)
                    {
                        SetText(AutoScrollText[count]);
                        count++;
                    }
                    else
                    {
                        count = 0;
                        SetText(AutoScrollText[count]);
                    }
                }
                else if (Loop == LoopMode.Shuffle)
                {
                    Random rand = new();
                    SetText(AutoScrollText[rand.Next(0, AutoScrollText.Count - 1)]);
                }
            }
            timer.Start();
        }

        public void SetScroller(List<string> strings, LoopMode mode)
        {
            timer.Stop();

            AutoScrollText = strings;
            Loop = mode;

            if (mode == LoopMode.Normal)
            {
                count = 0;
            }
            timer.Start();
        }

        public async void SetText(string? text)
        {
            await ContentTextBlock.Dispatcher.Invoke(async () =>
            {
                if (string.IsNullOrEmpty(text)) return;

                FormattedText formattedText = new FormattedText(
                    text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(ContentTextBlock.FontFamily, ContentTextBlock.FontStyle, ContentTextBlock.FontWeight, ContentTextBlock.FontStretch),
                    FontSize,
                    System.Windows.Media.Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                ContentTextBlock.Text = text;

                Open.To = formattedText.Width;
                BoardOpen.Begin();

                int delay = 0;
                if (text.Length >= 10)
                {
                    delay = text.Length * 200;
                }
                await Task.Delay(delay);

                Close.From = formattedText.Width;
                BoardClose.Begin();
            });
        }

    }
}
