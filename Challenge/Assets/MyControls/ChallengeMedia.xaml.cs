using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChallengeApp
{
    public partial class ChallengeMedia : UserControl
    {
        //public bool ImageLoaded { get { return (bool)GetValue(imageLoaded); } private set { SetValue(imageLoaded, value); } }
        //public static readonly DependencyProperty imageLoaded = DependencyProperty.Register("ImageLoaded", typeof(bool), typeof(UserControl), new PropertyMetadata(null));

        //public bool ImageFailed { get { return (bool)GetValue(imageFailed); } private set { SetValue(imageFailed, value); } }
        //public static readonly DependencyProperty imageFailed = DependencyProperty.Register("ImageFailed", typeof(bool), typeof(UserControl), new PropertyMetadata(null));

        public ChallengeMedia()
        {
            InitializeComponent();

            //VisualStateManager.GoToState(this, "LoadingMedia", false); //LoadingMedia
            //PlaceHolder.Height = PlaceHolder.ActualWidth;

            //ImageLoaded = false;
            //ImageFailed = false;
        }

        //private void ImageContent_ImageOpened(object sender, RoutedEventArgs e)
        //{
        //    ImageLoaded = true;
        //    ImageFailed = false;
        //    VisualStateManager.GoToState(this, "MediaLoaded", false);
        //}

        //private void ImageContent_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        //{
        //    ImageLoaded = false;
        //    ImageFailed = true;
        //    VisualStateManager.GoToState(this, "MediaFailed", false);
        //}

        //private void BitmapImage_DownloadProgress(object sender, System.Windows.Media.Imaging.DownloadProgressEventArgs e)
        //{
        //    if (e.Progress == 100)
        //    {
        //        ImageLoaded = true;
        //        ImageFailed = false;
        //        VisualStateManager.GoToState(this, "MediaLoaded", false);
        //    }
        //    else if(ImageLoaded)
        //    {
        //        ImageLoaded = false;
        //        VisualStateManager.GoToState(this, "LoadingMedia", false);
        //    }
        //}
    }
}
