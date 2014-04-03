using System;
//using System.Collections.Generic;
using System.Diagnostics;
//using System.Linq;
//using System.Net;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Media;
//using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChallengeApp.Controllers;
using ChallengeApp.Models;
using ChallengeApp.Resources;

namespace ChallengeApp.Views.Private
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        public User UserProfileObject { get { return (User)GetValue(userProfileObject); } set { SetValue(userProfileObject, value); } }
        public static readonly DependencyProperty userProfileObject = DependencyProperty.Register("UserProfileObject", typeof(User), typeof(PhoneApplicationPage), new PropertyMetadata(null));

        public ProfilePage()
        {
            InitializeComponent();

            UserProfileObject = App.UserProfileObject;

            Loaded += Profile_Loaded;
        }

        void Profile_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("Profile page loaded.");
            LoadUserProfile();
        }

        async void LoadUserProfile()
        {
            UserProfileObject = await UserController.Instance.GetUserById(UserProfileObject.id);
        }
    }
}