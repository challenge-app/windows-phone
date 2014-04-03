using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChallengeApp
{
    public partial class MyButton : Button
    {
        public Brush PressedBackgroundBrush { get { return (Brush)GetValue(pressedBackgroundBrush); } set { SetValue(pressedBackgroundBrush, value); } }
        public static readonly DependencyProperty pressedBackgroundBrush = DependencyProperty.Register("PressedBackgroundBrush", typeof(Brush), typeof(Button), new PropertyMetadata(null));

        public Brush PressedForegroundBrush { get { return (Brush)GetValue(pressedForegroundBrush); } set { SetValue(pressedForegroundBrush, value); } }
        public static readonly DependencyProperty pressedForegroundBrush = DependencyProperty.Register("PressedForegroundBrush", typeof(Brush), typeof(Button), new PropertyMetadata(null));

        public MyButton()
        {
            DefaultStyleKey = typeof(Button);
            InitializeComponent();

            Loaded += MyButton_Loaded;
        }

        void MyButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (PressedBackgroundBrush == null) PressedBackgroundBrush = this.Background;
            if (PressedForegroundBrush == null) PressedForegroundBrush = this.Foreground;
        }
    }
}
