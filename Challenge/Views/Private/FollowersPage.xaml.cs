using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ChallengeApp.Models;
using ChallengeApp.Controllers;

namespace ChallengeApp
{
    public partial class FollowersPage : PhoneApplicationPage
    {
        public enum Tab : int { FOLLOWING, FOLLOWERS };

        public User UserObject { get { return (User)GetValue(userObject); } set { SetValue(userObject, value); } }
        public static readonly DependencyProperty userObject = DependencyProperty.Register("UserObject", typeof(User), typeof(PhoneApplicationPage), new PropertyMetadata(null));
        
        public FollowersPage()
        {
            InitializeComponent();

            UserObject = App.UserProfileObject;

            Loaded += FollowersPage_Loaded;
            Unloaded += FollowersPage_Unloaded;
        }

        void FollowersPage_Loaded(object sender, RoutedEventArgs e)
        {
            UserController.Instance.FollowedOrUnfollowedUser += Instance_FollowedOrUnfollowedUser;

            LoadUsersList();
        }

        void FollowersPage_Unloaded(object sender, RoutedEventArgs e)
        {
            UserController.Instance.FollowedOrUnfollowedUser -= Instance_FollowedOrUnfollowedUser;

            UnloadUsersList();
        }

        void Instance_FollowedOrUnfollowedUser(object sender, Utils.FollowedOrUnfollowedUserEventArgs e)
        {
            // update in following list
            var user = UserController.Instance.FindUserInFollowingList(e.UserID);
            if (user != null) user.IsFollowing = e.IsFollowing;

            // update in followers list
            user = UserController.Instance.FindUserInFollowersList(e.UserID);
            if (user != null) user.IsFollowing = e.IsFollowing;
        }

        async void LoadUsersList()
        {
            var followingList = await UserController.Instance.GetUserFollowingList(UserObject.id);
            var followersList = await UserController.Instance.GetUserFollowersList(UserObject.id);

            UserObject.followingList = followingList;
            UserObject.followersList = followersList;

            FollowingListBox.ItemsSource = UserObject.followingList;
            FollowersListBox.ItemsSource = UserObject.followersList;
        }

        void UnloadUsersList()
        {
            UserObject.followingList = null;
            UserObject.followersList = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            int tab;
            int.TryParse(NavigationContext.QueryString["tab"], out tab);
            FollowersPivot.SelectedIndex = tab;
        }
    }
}