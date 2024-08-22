using iNKORE.UI.WPF.Modern.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gear.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyWindowExtended : Window
    {
        public NotifyWindowExtended(string Message = "Content 内容")
        {
            InitializeComponent();

#if DEBUG
            DebugButton.Visibility = Visibility.Visible;
#endif
            ContentTextBlock.Text = Message;
            DebugButton_Click(new(), new());
        }

        private async void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            #region Initialize Animations
            Storyboard Open_Border = new(), Open_Lorem = new(),
                Close_Border = new(), Close_Lorem = new();

            DoubleAnimation Open_Height = new()
            {
                From = SystemParameters.PrimaryScreenHeight,
                To = 300,
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new BackEase() { Amplitude = 0 },
            };

            DoubleAnimation Open_Opacity = new()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(600),

                EasingFunction = new CubicEase(),
            };

            DoubleAnimation Open_Width = new()
            {
                From = 2700,
                To = 0,

                Duration = TimeSpan.FromMilliseconds(500),

                EasingFunction = new CircleEase(),
            };
            Storyboard.SetTargetProperty(Open_Height, new(HeightProperty));
            Storyboard.SetTargetProperty(Open_Opacity, new(OpacityProperty));
            Storyboard.SetTargetProperty(Open_Width, new(WidthProperty));
            Open_Border.Children.Add(Open_Height); Open_Border.Children.Add(Open_Opacity);
            Open_Lorem.Children.Add(Open_Width);

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

            DoubleAnimation Close_Width = new()
            {
                From = 0,
                To = 2700,

                Duration = TimeSpan.FromMilliseconds(700),

                EasingFunction = new BackEase()
                {
                    Amplitude = 0
                },
            };
            Storyboard.SetTargetProperty(Close_Height, new(HeightProperty));
            Storyboard.SetTargetProperty(Close_Opacity, new(OpacityProperty));
            Storyboard.SetTargetProperty(Close_Width, new(WidthProperty));
            Close_Border.Children.Add(Close_Height); Close_Border.Children.Add(Close_Opacity);
            Close_Lorem.Children.Add(Close_Width);
            #endregion

            Open_Border.Begin(MainBorder);
            Open_Lorem.Begin(MarginTextBlock);
            await Task.Delay(1000);
            Close_Border.Begin(MainBorder);
            Close_Lorem.Begin(MarginTextBlock);

            Close();
        }
    }
}
