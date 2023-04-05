﻿using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Toolkit.Windows
{
    /// <summary>
    /// NotifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyWindow : Window
    {
        Timer timer = new Timer();
        public NotifyWindow()
        {
            InitializeComponent();
            SetText("事件：启动");
            timer.Interval = 3000;//1800000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (App.ShowTimer)
            {
                SetText("花老师永远滴神！");
            }
            timer.Start();
        }

        public async void SetText(string? text)
        {
            await ContentTextBlock.Dispatcher.Invoke(async () =>
            {
                ContentTextBlock.Text = text;

                DoubleAnimation Open = new DoubleAnimation();
                Open.From = 0;
                Open.To = text.Length * ContentTextBlock.FontSize;
                Open.Duration = new Duration(TimeSpan.FromSeconds(3));

                ElasticEase ElasticEase = new ElasticEase();
                ElasticEase.Oscillations = 2;
                ElasticEase.Springiness = 5;
                Open.EasingFunction = ElasticEase;

                ContentTextBlock.BeginAnimation(TextBlock.WidthProperty, Open);

                DoubleAnimation Close = new DoubleAnimation();
                Close.From = text.Length * ContentTextBlock.FontSize;
                Close.To = 0;
                Close.Duration = new Duration(TimeSpan.FromSeconds(3));

                BackEase BackEase = new();
                Close.EasingFunction = BackEase;

                await Task.Delay(2000);

                ContentTextBlock.BeginAnimation(TextBlock.WidthProperty, Close);
            });
        }

    }
}
