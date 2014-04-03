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
using ChallengeApp.Controllers;
using ChallengeApp.Models;

namespace ChallengeApp
{
    public partial class FollowButton : UserControl
    {
        //public bool IsChecked { get { return (bool)GetValue(isChecked); } set { SetValue(isChecked, value); } }
        //public static readonly DependencyProperty isChecked = DependencyProperty.Register("IsChecked", typeof(bool), typeof(UserControl), new PropertyMetadata(null));
		
        public FollowButton()
        {
            InitializeComponent();
        }

        private void Follow(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null) return;

            //follow
            if (!user.IsFollowing) UserController.Instance.Follow(user);
        }

        private void Unfollow(object sender, RoutedEventArgs e)
        {
            var user = (sender as FrameworkElement).DataContext as User;
            if (user == null) return;

            //unfollow
            if (user.IsFollowing) UserController.Instance.Unfollow(user);
        }
    }
}
