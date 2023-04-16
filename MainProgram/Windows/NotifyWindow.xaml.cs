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

    public enum AnimationMode
    {
        Open,
        Close
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

        public NotifyWindow()
        {
            InitializeComponent();
            SetText("事件：启动");
            timer.Interval = 3000;//1800000;
            timer.Elapsed += Timer_Elapsed;
        }

        private Storyboard GetStoryBoard(AnimationMode mode, DisplayMode dmode, string text)
        {
            Storyboard storyboard = new();
            DoubleAnimation animation = new();

            FormattedText? formattedText = new(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(ContentTextBlock.FontFamily, ContentTextBlock.FontStyle, ContentTextBlock.FontWeight, ContentTextBlock.FontStretch),
                ContentTextBlock.FontSize,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            if (dmode == DisplayMode.Text)
            {
                Storyboard.SetTargetName(ContentTextBlock, ContentTextBlock.Name);

                formattedText = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(ContentTextBlock.FontFamily, ContentTextBlock.FontStyle, ContentTextBlock.FontWeight, ContentTextBlock.FontStretch),
                ContentTextBlock.FontSize,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);
            }
            else if (dmode == DisplayMode.Formula)
            {
                Storyboard.SetTargetName(FormulaViewBox, FormulaViewBox.Name);

                formattedText = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(ContentFormulaControl.FontFamily, ContentFormulaControl.FontStyle, ContentFormulaControl.FontWeight, ContentFormulaControl.FontStretch),
                18,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);
            }

            if (mode == AnimationMode.Open)
            {
                animation = new()
                {
                    From = 0,
                    To = formattedText.Width,
                    Duration = new Duration(TimeSpan.FromSeconds(3))
                };

                animation.EasingFunction = new ElasticEase()
                {
                    Oscillations = 2,
                    Springiness = 5
                };

                Storyboard.SetTargetProperty(animation, new(WidthProperty));
                storyboard.Children.Add(animation);
            }
            else if (mode == AnimationMode.Close)
            {
                animation = new()
                {
                    From = formattedText.Width,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(2))
                };

                animation.EasingFunction = new BackEase()
                {
                    EasingMode = EasingMode.EaseIn
                };
                storyboard.Children.Add(animation);

                Storyboard.SetTargetProperty(animation, new(WidthProperty));
            }

            return storyboard;
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
                timer.Interval = showText.Length * 100 + 50000;

                if (Mode == DisplayMode.Text)
                {
                    SetText(showText);
                }
                else if (Mode == DisplayMode.Formula)
                {
                    SetFormula(showText);
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
            timer.Interval = 1000;
            timer.Start();
        }

        private async void SetFormula(string formula)
        {
            if (string.IsNullOrEmpty(formula)) return;

            await ContentFormulaControl.Dispatcher.Invoke(async () =>
            {

                Storyboard BoardOpen = GetStoryBoard(AnimationMode.Open, DisplayMode.Formula, formula);
                ContentFormulaControl.Formula = formula;
                BoardOpen.Begin(FormulaViewBox);

                int delay = 5000;
                await Task.Delay(delay);

                Storyboard BoardClose = GetStoryBoard(AnimationMode.Close, DisplayMode.Formula, formula);
                BoardClose.Begin(FormulaViewBox);
                await Task.Delay(2000);
                FormulaViewBox.Width = 0;
            });
        }

        public async void SetText(string text)
        {
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

                Storyboard BoardOpen = GetStoryBoard(AnimationMode.Open, DisplayMode.Text, text);
                ContentTextBlock.Text = text;
                BoardOpen.Begin(ContentTextBlock);

                int delay = 3000;
                if (text.Length >= 10)
                {
                    delay = text.Length * 100 + 5000;
                }
                await Task.Delay(delay);

                Storyboard BoardClose = GetStoryBoard(AnimationMode.Close, DisplayMode.Text, text);
                BoardClose.Begin(ContentTextBlock);
                await Task.Delay(2000);
                ContentTextBlock.Width = 0;
            });
        }

    }
}
