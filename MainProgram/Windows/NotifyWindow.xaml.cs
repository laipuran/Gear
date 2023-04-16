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
using WpfMath.Controls;
using static System.Net.Mime.MediaTypeNames;

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

        Queue<string> formula = new(), text = new();

        public NotifyWindow()
        {
            InitializeComponent();
            EnqueueText("事件：启动");
            timer.Interval = 3000; //1800000;
            timer.Elapsed += Timer_Elapsed;
        }

        private List<Storyboard> GetAnimation(DisplayMode mode, string text)
        {
            DoubleAnimation open = new()
            {
                From = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3)),

                EasingFunction = new ElasticEase()
                {
                    Oscillations = 1.5,
                    Springiness = 3
                }
            };

            DoubleAnimation close = new()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(2)),

                EasingFunction = new BackEase()
                {
                    EasingMode = EasingMode.EaseIn
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

                open.To = 70;
                close.From = 70;
            }

            Storyboard Open = new(); Open.Children.Add(open);
            Storyboard Close = new(); Close.Children.Add(close);

            return new() { Open, Close };
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

            if (App.Loop == LoopMode.Normal)
            {
                count = 0;
            }
            timer.Interval = 100;
            timer.Start();
        }

        private async void EnqueueFormula(string formula)
        {
            if (string.IsNullOrEmpty(formula)) return;

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

        public async void EnqueueText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            await ContentTextBlock.Dispatcher.Invoke(async () =>
            {
                var boards = GetAnimation(DisplayMode.Text, text);
                Storyboard BoardOpen = boards[0];
                ContentTextBlock.Text = text;
                BoardOpen.Begin(ContentTextBlock);

                int delay = 3000;
                if (text.Length >= 10)
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
